#pragma once
#include "WaveformType.h"
#include "Waveform.h"

// A singleton representing the current patch setup

namespace WomAudioComponent
{
	[Windows::UI::Xaml::Data::BindableAttribute]
	public ref class Patch sealed : Windows::UI::Xaml::Data::INotifyPropertyChanged
	{
	private:
		static Patch^ Instance;
		Patch();

		// Backing stores for properties

		//   WaveformType 
		int oscillator1Waveform;
		//  WaveformType 
		int oscillator2Waveform;

		int oscillator1Transpose;
		int oscillator2Transpose;
		double oscillator2FineTune;

		double oscillator1Volume;
		double oscillator2Volume;

		bool filterOn;
		double filterCutoffFrequency;
		double filterEmphasis;
		double filterEnvelope;

		double filterAttack;
		double filterDecay;
		double filterSustain;
		double filterRelease;

		double loudnessAttack;
		double loudnessDecay;
		double loudnessSustain;
		double loudnessRelease;

		// Methods that raise events
		void OnPropertyChanged(Platform::String^ propertyName);
		void OnDynamicPropertyChanged();
		void OnFilterEffectPropertyChanged();

	public:
		static Patch^ Create();

		// Required event for INotifyPropertyChanged
		virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

		// Additional events for subsets of the properties
		event Windows::Foundation::EventHandler<int>^ DynamicPropertyChanged;
		event Windows::Foundation::EventHandler<int>^ FilterEffectPropertyChanged;

		property /* WaveformType */ int Oscillator1Waveform
		{
			/* WaveformType */ int get() { return oscillator1Waveform; }
			void set(/* WaveformType */ int value);
		}

		property /* WaveformType */ int Oscillator2Waveform
		{
			/* WaveformType */ int get() { return oscillator2Waveform; }
			void set(/* WaveformType */ int value);
		}

		property int Oscillator1Transpose
		{
			int get() { return oscillator1Transpose; }
			void set(int value);
		}

		property int Oscillator2Transpose
		{
			int get() { return oscillator2Transpose; }
			void set(int value);
		}

		property double Oscillator2FineTune
		{
			double get() { return oscillator2FineTune; }
			void set(double value);
		}

		property double Oscillator1Volume
		{
			double get() { return oscillator1Volume; }
			void set(double value);
		}

		property double Oscillator2Volume
		{
			double get() { return oscillator2Volume; }
			void set(double value);
		}

		property bool FilterOn
		{
			bool get() { return filterOn; }
			void set(bool value);
		}

		property double FilterCutoffFrequency
		{
			double get() { return filterCutoffFrequency; }
			void set(double value);
		}

		property double FilterEmphasis
		{
			double get() { return filterEmphasis; }
			void set(double value);
		}

		property double FilterEnvelope
		{
			double get() { return filterEnvelope; }
			void set(double value);
		}

		property double FilterAttack
		{
			double get() { return filterAttack; }
			void set(double value);
		}

		property double FilterDecay
		{
			double get() { return filterDecay; }
			void set(double value);
		}

		property double FilterSustain
		{
			double get() { return filterSustain; }
			void set(double value);
		}

		property double FilterRelease
		{
			double get() { return filterRelease; }
			void set(double value);
		}

		property double LoudnessAttack
		{
			double get() { return loudnessAttack; }
			void set(double value);
		}

		property double LoudnessDecay
		{
			double get() { return loudnessDecay; }
			void set(double value);
		}

		property double LoudnessSustain
		{
			double get() { return loudnessSustain; }
			void set(double value);
		}

		property double LoudnessRelease
		{
			double get() { return loudnessRelease; }
			void set(double value);
		}

		void Save(Platform::String^ container);
		void Load(Platform::String^ container);

		[Windows::Foundation::Metadata::DefaultOverload]
		int LoadValue(Windows::Foundation::Collections::IPropertySet^ appSettings, Platform::String^ key, int default);
		double LoadValue(Windows::Foundation::Collections::IPropertySet^ appSettings, Platform::String^ key, double default);
		bool LoadValue(Windows::Foundation::Collections::IPropertySet^ appSettings, Platform::String^ key, bool default);
		//WaveformType LoadValue(Windows::Foundation::Collections::IPropertySet^ appSettings, Platform::String^ key, WaveformType default);
	};
}

