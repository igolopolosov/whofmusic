#pragma once

#include "Oscillator.h"
#include "DynamicSynthVoiceParameters.h"
#include "DynamicFilterParameters.h"
#include "TriggeredSynthVoiceParameters.h"
#include "FilterEnvelopeEffect.h"
#include "FilterEnvelopeParameters.h"
#include "AmplitudeEnvelopeEffect.h"
#include "AmplitudeEnvelopeParameters.h"

class SynthVoice
{
private:
    enum Effect
    {
        FilterEnvelope,
        AmplitudeEnvelope
    };

    Oscillator * pOscillator1;
    Oscillator * pOscillator2;
    IXAudio2SubmixVoice * pSubmixVoice;

    float triggerFrequency;
    int oscillator1Transpose;
    int oscillator2Transpose;
    float oscillator2FineTune;

    AmplitudeEnvelopeParameters amplitudeParams;
    FilterEnvelopeParameters filterParams;

public:
    SynthVoice(IXAudio2* pXAudio2, IXAudio2Voice* pDestinationVoice);

    void SetParameters(const DynamicSynthVoiceParameters * pParams);
    void SetFilterParameters(const DynamicFilterParameters * pParams);
    void Trigger(const TriggeredSynthVoiceParameters * pSynthVoiceParams);
    void Release();
    void SetOscillatorFrequencies();
};


