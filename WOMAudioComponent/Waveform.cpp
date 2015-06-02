#include "pch.h"
#include "Waveform.h"

using namespace WomAudioComponent;

using namespace DirectX;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml::Media;

Waveform::Waveform() : TOP(0), BASE(10), BOT(20), ZERO(0), PER(20)
{
}

PointCollection^ Waveform::Points::get()
{
    PointCollection^ pts = ref new PointCollection();

    switch (this->Type)
    {
    case WaveformType::Sine:
        for (int x = 0; x < 2 * PER; x++)
            pts->Append(Point(float(x), TOP + BASE * (1 - XMScalarSin(XM_2PI * x / PER))));

        break;

    case WaveformType::Triangle:
        pts->Append(Point(ZERO, BASE));
        pts->Append(Point(PER / 4, TOP));
        pts->Append(Point(3 * PER / 4, BOT));
        pts->Append(Point(5 * PER / 4, TOP));
        pts->Append(Point(7 * PER / 4, BOT));
        pts->Append(Point(2 * PER, BASE));
        break;

    case WaveformType::Sawtooth:
        pts->Append(Point(ZERO, BASE));
        pts->Append(Point(PER / 2, TOP));
        pts->Append(Point(PER / 2, BOT));
        pts->Append(Point(3 * PER / 2, TOP));
        pts->Append(Point(3 * PER / 2, BOT));
        pts->Append(Point(2 * PER, BASE));
        break;

    case WaveformType::Square:
        pts->Append(Point(ZERO, BASE));
        pts->Append(Point(ZERO, TOP));
        pts->Append(Point(PER / 2, TOP));
        pts->Append(Point(PER / 2, BOT));
        pts->Append(Point(PER, BOT));
        pts->Append(Point(PER, TOP));
        pts->Append(Point(3 * PER / 2, TOP));
        pts->Append(Point(3 * PER / 2, BOT));
        pts->Append(Point(2 * PER, BOT));
        pts->Append(Point(2 * PER, BASE));
        break;

    }
    return pts;
}
