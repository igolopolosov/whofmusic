#pragma once

namespace WomAudioComponent
{
	public ref struct KeyPressedArgs sealed
	{
		property int KeyNumber;
		property bool IsPressed;
		property WaveformType Instrument;
	};
}
