// AudioController.cpp
#include "pch.h"
#include "AudioController.h"

using namespace WOMAudioComponent;
using namespace Platform;

AudioController::AudioController(int numOscillators) : 
                        m_numOscillators(numOscillators)
{
    // Create an IXAudio2 object
    HRESULT hr = XAudio2Create(&m_xaudio2);

    if (FAILED(hr))
        ref new COMException(hr, "XAudio2Create failure");

    XAUDIO2_DEBUG_CONFIGURATION debugConfig = { 0 };
    debugConfig.TraceMask = XAUDIO2_LOG_DETAIL | XAUDIO2_LOG_WARNINGS;
    m_xaudio2->SetDebugConfiguration(&debugConfig);

    // Create a mastering voice
    hr = m_xaudio2->CreateMasteringVoice(&m_pMasteringVoice);

    if (FAILED(hr))
        ref new COMException(hr, "CreateMasteringVoice failure");

    // Create half a dozen oscillators
    for (int i = 0; i < numOscillators; i++)
    {
        SawtoothOscillator* oscillator = new SawtoothOscillator(m_xaudio2.Get());
        oscillator->SetAmplitude(0);
        m_availableOscillators.push_back(oscillator);
    }
}

AudioController::~AudioController()
{
    for (std::pair<int, SawtoothOscillator *> pair : m_playingOscillators)
    {
        NoteOff(pair.first);
    }

    for (SawtoothOscillator * oscillator : m_availableOscillators)
    {
        delete oscillator;
    }
}

void AudioController::Start()
{
    m_xaudio2->StartEngine();
}

void AudioController::Stop()
{
    m_xaudio2->StopEngine();
}

void AudioController::NoteOn(int id, int midiNumber)
{
    if (m_availableOscillators.size() > 0)
    {
        SawtoothOscillator* pOscillator = m_availableOscillators.back();
        m_availableOscillators.pop_back();

        double freq = 440 * pow(2, (midiNumber - 69) / 12.0);
        pOscillator->SetFrequency((float)freq);
        pOscillator->SetAmplitude(1.0f / m_numOscillators);
        m_playingOscillators[id] = pOscillator;
    }
}

void AudioController::NoteOff(int id)
{
    SawtoothOscillator * pOscillator = m_playingOscillators[id];

    if (pOscillator != nullptr)
    {
        pOscillator->SetAmplitude(0);
        m_availableOscillators.push_back(pOscillator);
        m_playingOscillators.erase(id);
    }
}
