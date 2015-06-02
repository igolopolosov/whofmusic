#include "pch.h"
#include "Oscillator.h"

using namespace DirectX;
using namespace Platform;

Oscillator::Oscillator(IXAudio2* pXAudio2, IXAudio2Voice* pDestinationVoice) : waveformType(WomAudioComponent::WaveformType::Triangle),
                                                                               index(0),
                                                                               angle(0.0f),
                                                                               angleIncrement(0.0f)
{
    // Define WAVEFORMATEX for a source voice
    WAVEFORMATEX waveFormat;
    waveFormat.wFormatTag = WAVE_FORMAT_IEEE_FLOAT;
    waveFormat.nChannels = 1;
    waveFormat.nSamplesPerSec = SAMPLE_FREQ;
    waveFormat.nAvgBytesPerSec = waveFormat.nSamplesPerSec * sizeof(float);
    waveFormat.wBitsPerSample = 8 * sizeof(float);
    waveFormat.nBlockAlign = waveFormat.nChannels * waveFormat.wBitsPerSample / 8;
    waveFormat.cbSize = 0;

    // Route this voice to the destination voice
    XAUDIO2_SEND_DESCRIPTOR sendDescriptor;
    sendDescriptor.Flags = 0;
    sendDescriptor.pOutputVoice = pDestinationVoice;

    XAUDIO2_VOICE_SENDS sendList;
    sendList.SendCount = 1;
    sendList.pSends = &sendDescriptor;

    // Create a source voice
    HRESULT hresult = pXAudio2->CreateSourceVoice(&pSourceVoice, &waveFormat,
                                                  XAUDIO2_VOICE_NOPITCH, 1.0f, this, &sendList); 
    if (FAILED(hresult))
        throw ref new COMException(hresult, "CreateSourceVoice failure");

    // Start the voice playing
    pSourceVoice->Start();
}

Oscillator::~Oscillator()
{
    pSourceVoice->Stop();
}

void Oscillator::SetFrequency(float frequency)
{
    angleIncrement = frequency / SAMPLE_FREQ;
}

void Oscillator::SetAmplitude(float amplitude)
{
    pSourceVoice->SetVolume(amplitude);
}

void Oscillator::SetWaveform(WomAudioComponent::WaveformType type)
{
    this->waveformType = type;
}

void _stdcall Oscillator::OnVoiceProcessingPassStart(UINT32 bytesRequired)
{
    if (bytesRequired == 0)
        return;

    // bytesRequired is always for 10 msec, so it equals 4 * 480.
    //      Because that's the value of BUFFER_LENGTH,
    //      startIndex should always be zero and
    //      endIndex should always equal BUFFER_LENGTH.

    int startIndex = index;
    int endIndex = startIndex + bytesRequired / sizeof(float);

    if (endIndex <= BUFFER_LENGTH)
    {
        FillAndSubmit(startIndex, endIndex - startIndex);
    }
    else
    {
        // Because the BUFFER_LENGTH is set to 10 msec, 
        //  this code should never be reached
        FillAndSubmit(startIndex, BUFFER_LENGTH - startIndex);
        FillAndSubmit(0, endIndex % BUFFER_LENGTH);
    }

    // Should set index back to 0
    index = endIndex % BUFFER_LENGTH;
}

void Oscillator::FillAndSubmit(int startIndex, int count)
{
    // Improve performance by moving for loop
    //  inside each case?

    for (int i = startIndex; i < startIndex + count; i++)
    {
        switch (waveformType)
        {
        case WomAudioComponent::WaveformType::Sine:
            waveformBuffer[i] = XMScalarSin(XM_2PI * angle);
            break;

		case WomAudioComponent::WaveformType::Triangle:
            waveformBuffer[i] = angle < 0.25f ? 4 * angle : (angle < 0.75f ? 1 - 4 * (angle - 0.25f) : 4 * (angle - 0.75f) - 1);
            break;

        case WomAudioComponent::WaveformType::Sawtooth:
            waveformBuffer[i] = 2 * (angle < 0.5f ? angle : angle - 1);
            break;

        case WomAudioComponent::WaveformType::Square:
            waveformBuffer[i] = angle < 0.5f ? 1.0f : -1.0f;
            break;

        default:
            waveformBuffer[i] = 0;
            break;
        }

        angle += angleIncrement;

        while (angle > 1)
            angle -= 1;
    }

    XAUDIO2_BUFFER buffer = {0};
    buffer.AudioBytes = sizeof(float) * BUFFER_LENGTH;
    buffer.pAudioData = (byte *)waveformBuffer.data();
    buffer.Flags = 0;
    buffer.PlayBegin = startIndex;
    buffer.PlayLength = count;
    
    HRESULT hresult = pSourceVoice->SubmitSourceBuffer(&buffer);

    if (FAILED(hresult))
        throw ref new COMException(hresult, "pSourceVoice->SubmitSourceBuffer");
}
