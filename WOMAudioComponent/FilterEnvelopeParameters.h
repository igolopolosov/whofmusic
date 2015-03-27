#pragma once
#include "EnvelopeGeneratorParameters.h"

struct FilterEnvelopeParameters
{
    bool isTriggered;

    // members for isTriggered == false
    float filterCutoffFrequency;
    float filterEmphasis;

    // members for isTriggered = true
    bool keyPressed;
    EnvelopeGeneratorParameters envelopeParams;
};