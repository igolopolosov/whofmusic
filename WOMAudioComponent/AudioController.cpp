// AudioController.cpp
#include "pch.h"
#include "AudioController.h"

using namespace Platform;
using namespace WomAudioComponent;
using namespace Windows::Foundation;

#define NUM_OSCILLATORS 10

AudioController::AudioController()
{
	// Create an IXAudio2 object
	HRESULT hresult = XAudio2Create(&pXAudio2);

	if (FAILED(hresult))
		ref new COMException(hresult, "XAudio2Create failure");

	// Set for debugging
	XAUDIO2_DEBUG_CONFIGURATION debugConfig = { 0 };
	debugConfig.TraceMask = XAUDIO2_LOG_DETAIL | XAUDIO2_LOG_WARNINGS;
	pXAudio2->SetDebugConfiguration(&debugConfig);

	// Create a mastering voice
	hresult = pXAudio2->CreateMasteringVoice(&pMasteringVoice);

	if (FAILED(hresult))
		ref new COMException(hresult, "CreateMasteringVoice failure");

	// Volume meter effect
	IUnknown * pVolumeMeterAPO;
	XAudio2CreateVolumeMeter(&pVolumeMeterAPO);

	// Reference the effect with two structures
	XAUDIO2_EFFECT_DESCRIPTOR effectDescriptor;
	effectDescriptor.pEffect = pVolumeMeterAPO;
	effectDescriptor.InitialState = true;
	effectDescriptor.OutputChannels = 2;

	XAUDIO2_EFFECT_CHAIN effectChain;
	effectChain.EffectCount = 1;
	effectChain.pEffectDescriptors = &effectDescriptor;

	// Set the effect on the mastering voice
	pMasteringVoice->SetEffectChain(&effectChain);

	// Release the local reference to the effect
	pVolumeMeterAPO->Release();

	// Create a sufficient number of oscillators
	for (int i = 0; i < NUM_OSCILLATORS; i++)
	{
		SynthVoice * pSynthVoice = new SynthVoice(pXAudio2.Get(), pMasteringVoice);
		allSynthVoices.push_back(pSynthVoice);
		availableSynthVoices.push_back(pSynthVoice);
	}
}

AudioController::~AudioController()
{
	for (std::pair<int, SynthVoice *> pair : playingSynthVoices)
	{
		if (pair.second != nullptr)
		{
			// Signal a release
			pair.second->Release();

			// At this point, the key has been released but the voice is not
			//  yet completed because the release segment of the envelope
			//  is still in progress.

			// For that reason, put the voice at the beginning of the 
			//  collection rather than the end to avoid it being reused
			//  immediately.
			availableSynthVoices.insert(availableSynthVoices.begin(), pair.second);

			// Remove it from playingSynthVoices collection
			playingSynthVoices.erase(pair.first);
		}
	}

	for (SynthVoice * voice : availableSynthVoices)
	{
		delete voice;
	}
}

void AudioController::OnDynamicPropertyChanged(Object^ sender, int args)
{
	DynamicSynthVoiceParameters params;

	params.oscillator1Waveform = (WaveformType)patch->Oscillator1Waveform;
	params.oscillator2Waveform = (WaveformType)patch->Oscillator2Waveform;

	params.oscillator1Transpose = patch->Oscillator1Transpose;
	params.oscillator2Transpose = patch->Oscillator2Transpose;
	params.oscillator2FineTune = float(pow(2.0, patch->Oscillator2FineTune / 12));

	params.oscillator1Amplitude = float(patch->Oscillator1Volume / 100);
	params.oscillator2Amplitude = float(patch->Oscillator2Volume / 100);

	for (SynthVoice * pSynthVoice : allSynthVoices)
		pSynthVoice->SetParameters(&params);
}

void AudioController::OnFilterEffectPropertyChanged(Object^ sender, int args)
{
	DynamicFilterParameters params;
	params.filterOn = patch->FilterOn;
	params.filterCutoffFrequency = float(patch->FilterCutoffFrequency);
	params.filterEmphasis = float(patch->FilterEmphasis);

	for (SynthVoice * pSynthVoice : allSynthVoices)
		pSynthVoice->SetFilterParameters(&params);
}

void AudioController::KeyIsPressedChanged(Object^ sender, KeyPressedArgs^ args)
{
	int keyNum = args->KeyNumber;

	if (args->IsPressed)
	{
		if (availableSynthVoices.size() > 0)
		{
			// Get SynthVoice from end of availableSynthVoices
			SynthVoice * pSynthVoice = availableSynthVoices.back();
			availableSynthVoices.pop_back();

			DynamicSynthVoiceParameters dynamic;
			dynamic.oscillator1Waveform = (WaveformType)args->Instrument;
			dynamic.oscillator2Waveform = (WaveformType)args->Instrument;

			dynamic.oscillator1Transpose = patch->Oscillator1Transpose;
			dynamic.oscillator2Transpose = patch->Oscillator2Transpose;
			dynamic.oscillator2FineTune = float(pow(2.0, patch->Oscillator2FineTune / 12));

			dynamic.oscillator1Amplitude = float(patch->Oscillator1Volume / 100);
			dynamic.oscillator2Amplitude = float(patch->Oscillator2Volume / 100);
			pSynthVoice->SetParameters(&dynamic);

			// Set up a TriggeredSynthVoiceParameters structure
			TriggeredSynthVoiceParameters params;
			params.triggerFrequency = float(440 * pow(2, (keyNum % 1000 - 69) / 12.0));

			// Amplitude envelope
			float attackTime = float(pow(10, patch->LoudnessAttack));
			float decayTime = float(pow(10, patch->LoudnessDecay));
			float sustainLevel = float(patch->LoudnessSustain / 100);
			float releaseTime = float(pow(10, patch->LoudnessRelease));

			params.amplitudeEnvelope.baseLevel = 0;
			params.amplitudeEnvelope.attackRate = float(1 / attackTime);
			params.amplitudeEnvelope.peakLevel = 1;
			params.amplitudeEnvelope.decayRate = float((sustainLevel - 1) / decayTime);
			params.amplitudeEnvelope.sustainLevel = sustainLevel;
			params.amplitudeEnvelope.releaseTime = releaseTime;

			// Filter parameters
			params.filterParameters.filterOn = patch->FilterOn;
			params.filterParameters.filterCutoffFrequency = float(patch->FilterCutoffFrequency);
			params.filterParameters.filterEmphasis = float(patch->FilterEmphasis);

			// Filter envelope
			float baseLevel = 1;
			float peakLevel = baseLevel + float(patch->FilterEnvelope / 6.67);        // ie, 4 octaves
			sustainLevel = baseLevel + float(patch->FilterEnvelope * patch->FilterSustain / 667);

			attackTime = float(pow(10, patch->FilterAttack));
			decayTime = float(pow(10, patch->FilterDecay));
			releaseTime = float(pow(10, patch->FilterRelease));

			params.filterEnvelope.baseLevel = baseLevel;
			params.filterEnvelope.attackRate = float((peakLevel - baseLevel) / attackTime);
			params.filterEnvelope.peakLevel = peakLevel;
			params.filterEnvelope.decayRate = float((sustainLevel - peakLevel) / decayTime);
			params.filterEnvelope.sustainLevel = sustainLevel;
			params.filterEnvelope.releaseTime = releaseTime;

			// Trigger the voice
			pSynthVoice->Trigger(&params);

			// Add the voice to the "playing" collection
			playingSynthVoices[keyNum] = pSynthVoice;
		}
	}
	else
	{
		// Get the playing SynthVoice
		SynthVoice * pSynthVoice = playingSynthVoices[keyNum];

		if (pSynthVoice != nullptr)
		{
			// Signal a release
			pSynthVoice->Release();

			// At this point, the key has been released but the voice is not
			//  yet completed because the release segment of the envelope
			//  is still in progress.

			// For that reason, put the voice at the beginning of the 
			//  collection rather than the end to avoid it being reused
			//  immediately.
			availableSynthVoices.insert(availableSynthVoices.begin(), pSynthVoice);

			// Remove it from playingSynthVoices collection
			playingSynthVoices.erase(keyNum);
		}
	}
}

void AudioController::Start()
{
	pXAudio2->StartEngine();
}

void AudioController::ReleaseVoices()
{
	return;
	for (std::pair<int, SynthVoice *> pair : playingSynthVoices)
	{
		if (pair.second != nullptr)
		{
			// Signal a release
			pair.second->Release();

			// At this point, the key has been released but the voice is not
			//  yet completed because the release segment of the envelope
			//  is still in progress.

			// For that reason, put the voice at the beginning of the 
			//  collection rather than the end to avoid it being reused
			//  immediately.
			availableSynthVoices.insert(availableSynthVoices.begin(), pair.second);

			// Remove it from playingSynthVoices collection
			playingSynthVoices.erase(pair.first);
		}
	}

	for (SynthVoice * voice : availableSynthVoices)
	{
		delete voice;
	}
	// Create a sufficient number of oscillators
	for (int i = 0; i < NUM_OSCILLATORS; i++)
	{
		SynthVoice * pSynthVoice = new SynthVoice(pXAudio2.Get(), pMasteringVoice);
		allSynthVoices.push_back(pSynthVoice);
		availableSynthVoices.push_back(pSynthVoice);
	}
}

void AudioController::Stop()
{
	pXAudio2->StopEngine();
}

void AudioController::SetVolumeLevel(int volumeLevel)
{
	HRESULT hresult = pMasteringVoice->SetVolume(float(volumeLevel) / 100);

	if (FAILED(hresult))
		ref new COMException(hresult, "pMasteringVoice->SetVolume failure");
}

void AudioController::CreatePatch()
{
	// Create a singleton Patch object
	patch = Patch::Create();

	// Set event handlers for dynamic property subsets
	patch->DynamicPropertyChanged += ref new EventHandler<int>(this, &AudioController::OnDynamicPropertyChanged);
	patch->FilterEffectPropertyChanged += ref new EventHandler<int>(this, &AudioController::OnFilterEffectPropertyChanged);

	// Get the saved patch & set to DataContext
	patch->Load(nullptr);
}

Patch^ AudioController::GetPatch()
{
	return patch;
}

bool AudioController::IsOverloadPeaks()
{
	float peakLevels[2];
	float rmsLevels[2];
	XAUDIO2FX_VOLUMEMETER_LEVELS volumeMeter;
	volumeMeter.pPeakLevels = peakLevels;
	volumeMeter.pRMSLevels = rmsLevels;
	volumeMeter.ChannelCount = 2;
	pMasteringVoice->GetEffectParameters(0, &volumeMeter, sizeof(volumeMeter));
	return (peakLevels[0] > 1 || peakLevels[1] > 1);
}