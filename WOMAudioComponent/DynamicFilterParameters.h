#pragma once

// These are the filter effect parameters than affect
//      the voice as it's playing, but also must be part
//      of the TriggeredSynthVoiceParameters, which apply 
//      when a voice is triggered

struct DynamicFilterParameters
{
    bool filterOn;
    float filterCutoffFrequency;
    float filterEmphasis;
};