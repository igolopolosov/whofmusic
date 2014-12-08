#pragma once

namespace WOMAudioComponent
{
    class SawtoothOscillator
    {
    private:
        static const int BASE_FREQ = 20;
        static const int BUFFER_LENGTH = (44100 / BASE_FREQ);

        IXAudio2SourceVoice * pSourceVoice;
        short waveformBuffer[BUFFER_LENGTH];

    public:
        SawtoothOscillator(IXAudio2* pXAudio2);
        void SetFrequency(float freq);
        void SetAmplitude(float amp);
    };
}

