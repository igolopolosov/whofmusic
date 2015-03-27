#include "pch.h"
#include "EnvelopeGenerator.h"

EnvelopeGenerator::EnvelopeGenerator() : state(State::Dormant)
{
    params.baseLevel = 0;
}

void EnvelopeGenerator::SetParameters(const EnvelopeGeneratorParameters params)
{
    this->params = params;
}

void EnvelopeGenerator::Attack()
{
    state = State::Attack;
    level = params.baseLevel;
    isReleased = false;
}

void EnvelopeGenerator::Release()
{
    isReleased = true;
}

bool EnvelopeGenerator::GetNextValue(float interval, float& value)
{
    bool completed = false;

    // If note is released, go directly to Release state,
    //      except if still attacking
    if (isReleased && 
        (state == State::Decay || state == State::Sustain))
    {
        state = State::Release;
        releaseRate = (params.baseLevel - level) / params.releaseTime;
    }

    switch (state)
    {
    case State::Dormant:
        level = params.baseLevel;
        completed = true;
        break;

    case State::Attack:
        level += interval * params.attackRate;

        if ((params.attackRate > 0 && level >= params.peakLevel) ||
            (params.attackRate < 0 && level <= params.peakLevel))
        {
            level = params.peakLevel;
            state = State::Decay;
        }
        break;

    case State::Decay:
        level += interval * params.decayRate;

        if ((params.decayRate > 0 && level >= params.sustainLevel) ||
            (params.decayRate < 0 && level <= params.sustainLevel))
        {
            level = params.sustainLevel;
            state = State::Sustain;
        }
        break;

    case State::Sustain:
        break;

    case State::Release:
        level += interval * releaseRate;

        if ((releaseRate > 0 && level >= params.baseLevel) ||
            (releaseRate < 0 && level <= params.baseLevel))
        {
            level = params.baseLevel;
            state = State::Dormant;
            completed = true;
        }
    }

    value = level;
    return completed;
}

