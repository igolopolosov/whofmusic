#pragma once
#include "SawtoothOscillator.h"

namespace WOMAudioComponent
{
    public ref class AudioController sealed
    {
    private:
        ~AudioController();
        int m_numOscillators;
        Microsoft::WRL::ComPtr<IXAudio2> m_xaudio2;
        IXAudio2MasteringVoice * m_pMasteringVoice;
        std::vector<SawtoothOscillator *> m_availableOscillators;
        std::map<int, SawtoothOscillator *> m_playingOscillators;

    public:
        AudioController(int numOscillators);
        void Start();
        void Stop();
        void NoteOn(int id, int midiNumber);
        void NoteOff(int id);
    };
}