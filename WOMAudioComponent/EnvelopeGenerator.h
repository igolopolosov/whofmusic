#pragma once
#include "EnvelopeGeneratorParameters.h"

class EnvelopeGenerator
{
private:
    enum class State
    {
        Dormant, Attack, Decay, Sustain, Release
    };

    EnvelopeGeneratorParameters params;
    float level;
    State state;
    bool isReleased;
    float releaseRate;

public:
    EnvelopeGenerator();
    void SetParameters(const EnvelopeGeneratorParameters params);
    void Attack();
    void Release();
    bool GetNextValue(float interval, float& value);
};