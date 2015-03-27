#pragma once
#include "WaveformType.h"

class Oscillator : public IXAudio2VoiceCallback
{
private:
    static const int SAMPLE_FREQ = 48000;
    static const int BUFFER_LENGTH = SAMPLE_FREQ / 100;

    IXAudio2SourceVoice * pSourceVoice;
    std::array<float, BUFFER_LENGTH> waveformBuffer;
    int index;
    float angle;
    float angleIncrement;
    WomAudioComponent::WaveformType waveformType;

public:
    Oscillator(IXAudio2* pXAudio2, IXAudio2Voice* pDestinationVoice);
    ~Oscillator();

    void SetFrequency(float frequency);
    void SetAmplitude(float amplitude);
    void SetWaveform(WomAudioComponent::WaveformType type);

    // Callbacks required for IXAudio2VoiceCallback
    void _stdcall OnVoiceProcessingPassStart(UINT32 bytesRequired);
    void _stdcall OnVoiceProcessingPassEnd(){}
    void _stdcall OnStreamEnd(){}
    void _stdcall OnBufferStart(void* pContext){};
    void _stdcall OnBufferEnd(void* pContext){};
    void _stdcall OnLoopEnd(void*){}
    void _stdcall OnVoiceError(void*, HRESULT){}

private:
    void FillAndSubmit(int startIndex, int count);
};

