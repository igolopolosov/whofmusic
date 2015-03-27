#pragma once

struct EnvelopeGeneratorParameters
{
    float baseLevel;
    float attackRate;   // in level/msec
    float peakLevel;
    float decayRate;    // in level/msec
    float sustainLevel;
    float releaseTime;  // in msec
};
