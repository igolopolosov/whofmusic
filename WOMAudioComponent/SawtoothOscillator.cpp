#include "pch.h"
#include "SawtoothOscillator.h"

using namespace WOMAudioComponent;
using namespace Platform;

SawtoothOscillator::SawtoothOscillator(IXAudio2* pXAudio2)
{
    // Create a source voice
    WAVEFORMATEX waveFormat;
    waveFormat.wFormatTag = WAVE_FORMAT_PCM;
    waveFormat.nChannels = 1;
    waveFormat.nSamplesPerSec = 44100;
    waveFormat.nAvgBytesPerSec = 44100 * 2;
    waveFormat.nBlockAlign = 2;
    waveFormat.wBitsPerSample = 16;
    waveFormat.cbSize = 0;

    HRESULT hr = pXAudio2->CreateSourceVoice(&pSourceVoice, &waveFormat, 
                                             0, XAUDIO2_MAX_FREQ_RATIO);
    if (FAILED(hr))
        throw ref new COMException(hr, "CreateSourceVoice failure");

    // Initialize the array
	for (int sample = 0; sample < BUFFER_LENGTH; sample++)
	{
		if (sample < BUFFER_LENGTH / 2)
		{
			waveformBuffer[sample] = (short)(65535 * sample / BUFFER_LENGTH - 32768);
		}
		else 
		{
			waveformBuffer[sample] = (short)(65535 * (BUFFER_LENGTH - sample) / BUFFER_LENGTH - 32768);
		}
	}

    // Submit the array
    XAUDIO2_BUFFER buffer = {0};
    buffer.AudioBytes = 2 * BUFFER_LENGTH;
    buffer.pAudioData = (byte *)waveformBuffer;
    buffer.Flags = XAUDIO2_END_OF_STREAM;
    buffer.PlayBegin = 0;
    buffer.PlayLength = BUFFER_LENGTH;
    buffer.LoopBegin = 0;
    buffer.LoopLength = BUFFER_LENGTH;
    buffer.LoopCount = XAUDIO2_LOOP_INFINITE;

    hr = pSourceVoice->SubmitSourceBuffer(&buffer);

    if (FAILED(hr))
        throw ref new COMException(hr, "SubmitSourceBuffer failure");

    // Start the voice playing
    pSourceVoice->Start();
}

void SawtoothOscillator::SetFrequency(float freq)
{
    pSourceVoice->SetFrequencyRatio(freq / BASE_FREQ);
}

void SawtoothOscillator::SetAmplitude(float amp)
{
    pSourceVoice->SetVolume(amp);
}
