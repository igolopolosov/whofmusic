#include "pch.h"
#include "SynthVoice.h"

using namespace Microsoft::WRL;
using namespace Platform;

SynthVoice::SynthVoice(IXAudio2* pXAudio2, IXAudio2Voice* pDestinationVoice) : triggerFrequency(0),
                                                                               oscillator1Transpose(0),
                                                                               oscillator2Transpose(0),
                                                                               oscillator2FineTune(1)
{
    // Route this voice to the destination voice
    XAUDIO2_SEND_DESCRIPTOR sendDescriptor;
    sendDescriptor.Flags = 0;
    sendDescriptor.pOutputVoice = pDestinationVoice;

    XAUDIO2_VOICE_SENDS sendList;
    sendList.SendCount = 1;
    sendList.pSends = &sendDescriptor;

    // Create the submix voice for this voice
    HRESULT hresult = pXAudio2->CreateSubmixVoice(&pSubmixVoice, 2, 48000, 0, 1, &sendList);

    if (FAILED(hresult))
        throw ref new COMException(hresult, "CreateSubmixVoice failure");

    // Create a pair of oscillators
    pOscillator1 = new Oscillator(pXAudio2, pSubmixVoice);
    pOscillator1->SetAmplitude(0);

    pOscillator2 = new Oscillator(pXAudio2, pSubmixVoice);
    pOscillator2->SetAmplitude(0);

    // Create filter envelope effect
    ComPtr<FilterEnvelopeEffect> pFilterEnvelopeEffect = FilterEnvelopeEffect::Create();

    // Create amplitude envelope effect
    ComPtr<AmplitudeEnvelopeEffect> pAmplitudeEnvelopeEffect = AmplitudeEnvelopeEffect::Create();

    // Reference those effects with an effect descriptor array
    XAUDIO2_EFFECT_DESCRIPTOR effectDescriptors[2];

    effectDescriptors[0].pEffect = (IUnknown *) (IXAPOParameters *) pFilterEnvelopeEffect.Get();
    effectDescriptors[0].InitialState = false;  // disabled based on default setting of toggle button
    effectDescriptors[0].OutputChannels = 2;

    effectDescriptors[1].pEffect = (IUnknown *) (IXAPOParameters *) pAmplitudeEnvelopeEffect.Get();
    effectDescriptors[1].InitialState = true;
    effectDescriptors[1].OutputChannels = 2;

    // Reference that array with a effect chain
    XAUDIO2_EFFECT_CHAIN effectChain;
    effectChain.EffectCount = 2;
    effectChain.pEffectDescriptors = effectDescriptors;
    hresult = pSubmixVoice->SetEffectChain(&effectChain);

    if (FAILED(hresult))
        throw ref new COMException(hresult, "pSubmixVoice->SetEffectChain failure");
}

void SynthVoice::SetParameters(const DynamicSynthVoiceParameters * pParams)
{
    pOscillator1->SetWaveform(pParams->oscillator1Waveform);
    pOscillator2->SetWaveform(pParams->oscillator2Waveform);

    // Set oscillator frequencies
    this->oscillator1Transpose = pParams->oscillator1Transpose;
    this->oscillator2Transpose = pParams->oscillator2Transpose;
    this->oscillator2FineTune = pParams->oscillator2FineTune;

    // Also sets filter cutoff frequency
    SetOscillatorFrequencies();

    // Set oscillator amplitudes
    pOscillator1->SetAmplitude(pParams->oscillator1Amplitude);
    pOscillator2->SetAmplitude(pParams->oscillator2Amplitude);
}

void SynthVoice::SetFilterParameters(const DynamicFilterParameters * pParams)
{
    HRESULT hresult = 0;

    if (pParams->filterOn)
        hresult = pSubmixVoice->EnableEffect(FilterEnvelope);
    else
        hresult = pSubmixVoice->DisableEffect(FilterEnvelope);
    
    if (FAILED(hresult))
        throw ref new COMException(hresult, "pSubmixVoice->Enable/DisableEffect");

    if (pParams->filterOn)
    {
        filterParams.isTriggered = false;

        // Adjust incoming filter-cutoff-frequency expressed as an 
        //      octave offset to an absolute frequency.
        filterParams.filterCutoffFrequency = triggerFrequency * float(pow(2, pParams->filterCutoffFrequency));
        filterParams.filterEmphasis = pParams->filterEmphasis;

        hresult = pSubmixVoice->SetEffectParameters(FilterEnvelope, &filterParams, sizeof(FilterEnvelopeParameters));

        if (FAILED(hresult))
            throw ref new COMException(hresult, "pSubmixVoice->SetEffectParameters(0)");
    }
}

void SynthVoice::Trigger(const TriggeredSynthVoiceParameters * pParams)
{
    // Set frequencies of two oscillators
    triggerFrequency = pParams->triggerFrequency;
    SetOscillatorFrequencies();

    // Set the filter envelope parameters
    filterParams.isTriggered = true;
    filterParams.keyPressed = true;

    // Adjust incoming filter-cutoff-frequency expressed as an 
    //      octave offset to an absolute frequency.
    filterParams.filterCutoffFrequency = triggerFrequency * float(pow(2, pParams->filterParameters.filterCutoffFrequency));
    filterParams.filterEmphasis = pParams->filterParameters.filterEmphasis;
    filterParams.envelopeParams = pParams->filterEnvelope;

    HRESULT hresult = pSubmixVoice->SetEffectParameters(FilterEnvelope, &filterParams, sizeof(FilterEnvelopeParameters));

    if (FAILED(hresult))
        throw ref new COMException(hresult, "pSubmixVoice->SetEffectParameters(0)");

    // Set the amplitude envelope parameters
    amplitudeParams.envelopeParams = pParams->amplitudeEnvelope;
    amplitudeParams.keyPressed = true;

    hresult = pSubmixVoice->SetEffectParameters(AmplitudeEnvelope, &amplitudeParams, sizeof(AmplitudeEnvelopeParameters));

    if (FAILED(hresult))
        throw ref new COMException(hresult, "pSubmixVoice->SetEffectParameters(1)");
}

void SynthVoice::SetOscillatorFrequencies()
{
    float frequency = triggerFrequency;
    frequency *= powf(2.0f, float(oscillator1Transpose));
    pOscillator1->SetFrequency(frequency);

    frequency = triggerFrequency;
    frequency *= powf(2.0f, float(oscillator2Transpose));
    frequency *= oscillator2FineTune;
    pOscillator2->SetFrequency(frequency);
}

void SynthVoice::Release()
{
    // Release filter envelope
    filterParams.isTriggered = true;
    filterParams.keyPressed = false;
    HRESULT hresult = pSubmixVoice->SetEffectParameters(FilterEnvelope, &filterParams, sizeof(FilterEnvelopeParameters));

    if (FAILED(hresult))
        throw ref new COMException(hresult, "pSubmixVoice->SetEffectParameters(0)");

    // Release amplitude envelope
    amplitudeParams.keyPressed = false;
    hresult = pSubmixVoice->SetEffectParameters(AmplitudeEnvelope, &amplitudeParams, sizeof(AmplitudeEnvelopeParameters));

    if (FAILED(hresult))
        throw ref new COMException(hresult, "pSubmixVoice->SetEffectParameters(1)");
}
