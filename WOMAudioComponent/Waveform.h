#pragma once
#include "WaveformType.h"

namespace WomAudioComponent
{
    [Windows::UI::Xaml::Data::BindableAttribute]
    public ref class Waveform sealed
    {
    private:
        // For drawing waveforms for combo boxes
        float TOP;
        float BASE;
        float BOT;
        float ZERO;
        float PER;     // period width

    public:
        Waveform();

        property WaveformType Type;

        property int IntType 
        { 
            int get() { return (int)this->Type; }
        }

        property Windows::UI::Xaml::Media::PointCollection^ Points
        {
            Windows::UI::Xaml::Media::PointCollection^ get();
        }
    };
}
