#pragma once
#include "DynamicFilterParameters.h"
#include "EnvelopeGeneratorParameters.h"

struct TriggeredSynthVoiceParameters
{
    float triggerFrequency;
    DynamicFilterParameters filterParameters;
    EnvelopeGeneratorParameters amplitudeEnvelope;
    EnvelopeGeneratorParameters filterEnvelope;
};