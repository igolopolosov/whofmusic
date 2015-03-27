#pragma once
#include "WaveformType.h"

// These are the synthesizer voice parameters than affect
//      the voice as it's playing, as opposed to the 
//      TriggeredSynthVoiceParameters, which apply when
//      a voice is triggered

struct DynamicSynthVoiceParameters
{
    WomAudioComponent::WaveformType oscillator1Waveform;
    WomAudioComponent::WaveformType oscillator2Waveform;

    int oscillator1Transpose;
    int oscillator2Transpose;
    float oscillator2FineTune;

    float oscillator1Amplitude;
    float oscillator2Amplitude;
};