#include "pch.h"
#include "Patch.h"

using namespace WomAudioComponent;

using namespace Platform;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml::Data;

Patch^ Patch::Instance = nullptr;

Patch^ Patch::Create()
{
    if (Patch::Instance == nullptr)
        Patch::Instance = ref new Patch();

    return Instance;
}

Patch::Patch()
{	
}

void Patch::OnPropertyChanged(String^ propertyName)
{
    //PropertyChanged(this, ref new PropertyChangedEventArgs(propertyName));
}

void Patch::OnFilterEffectPropertyChanged()
{
    FilterEffectPropertyChanged(this, 0);
}

void Patch::OnDynamicPropertyChanged()
{
    DynamicPropertyChanged(this, 0);
}

void Patch::Oscillator1Waveform::set(int value)
{
    if (oscillator1Waveform != value)
    {
        oscillator1Waveform = value;
        OnPropertyChanged("Oscillator1Waveform");
        OnDynamicPropertyChanged();
    }
}

void Patch::Oscillator2Waveform::set(int value)
{
    if (oscillator2Waveform != value)
    {
        oscillator2Waveform = value;
        OnPropertyChanged("Oscillator2Waveform");
        OnDynamicPropertyChanged();
    }
}

void Patch::Oscillator1Transpose::set(int value)
{
    if (oscillator1Transpose != value)
    {
        oscillator1Transpose = value;
        OnPropertyChanged("Oscillator1Transpose");
        OnDynamicPropertyChanged();
    }
}

void Patch::Oscillator2Transpose::set(int value)
{
    if (oscillator2Transpose != value)
    {
        oscillator2Transpose = value;
        OnPropertyChanged("Oscillator2Transpose");
        OnDynamicPropertyChanged();
    }
}

void Patch::Oscillator2FineTune::set(double value)
{
    if (oscillator2FineTune != value)
    {
        oscillator2FineTune = value;
        OnPropertyChanged("Oscillator2FineTune");
        OnDynamicPropertyChanged();
    }
}

void Patch::Oscillator1Volume::set(double value)
{
    if (oscillator1Volume != value)
    {
        oscillator1Volume = value;
        OnPropertyChanged("Oscillator1Volume");
        OnDynamicPropertyChanged();
    }
}

void Patch::Oscillator2Volume::set(double value)
{
    if (oscillator2Volume != value)
    {
        oscillator2Volume = value;
        OnPropertyChanged("Oscillator2Volume");
        OnDynamicPropertyChanged();
    }
}

void Patch::FilterOn::set(bool value)
{
    if (filterOn != value)
    {
        filterOn = value;
        OnPropertyChanged("FilterOn");
        OnFilterEffectPropertyChanged();
    }
}

void Patch::FilterCutoffFrequency::set(double value)
{
    if (filterCutoffFrequency != value)
    {
        filterCutoffFrequency = value;
        OnPropertyChanged("FilterCutoffFrequency");
        OnFilterEffectPropertyChanged();
    }
}

void Patch::FilterEmphasis::set(double value)
{
    if (filterEmphasis != value)
    {
        filterEmphasis = value;
        OnPropertyChanged("FilterEmphasis");
        OnFilterEffectPropertyChanged();
    }
}

void Patch::FilterEnvelope::set(double value)
{
    if (filterEnvelope != value)
    {
        filterEnvelope = value;
        OnPropertyChanged("FilterCutoffFrequency");
    }
}

void Patch::FilterAttack::set(double value)
{
    if (filterAttack != value)
    {
        filterAttack = value;
        OnPropertyChanged("FilterAttack");
    }
}

void Patch::FilterDecay::set(double value)
{
    if (filterDecay != value)
    {
        filterDecay = value;
        OnPropertyChanged("FilterDecay");
    }
}

void Patch::FilterSustain::set(double value)
{
    if (filterSustain != value)
    {
        filterSustain = value;
        OnPropertyChanged("FilterSustain");
    }
}

void Patch::FilterRelease::set(double value)
{
    if (filterRelease != value)
    {
        filterRelease = value;
        OnPropertyChanged("FilterRelease");
    }
}

void Patch::LoudnessAttack::set(double value)
{
    if (loudnessAttack != value)
    {
        loudnessAttack = value;
        OnPropertyChanged("LoudnessAttack");
    }
}

void Patch::LoudnessDecay::set(double value)
{
    if (loudnessDecay != value)
    {
        loudnessDecay = value;
        OnPropertyChanged("LoudnessDecay");
    }
}

void Patch::LoudnessSustain::set(double value)
{
    if (loudnessSustain != value)
    {
        loudnessSustain = value;
        OnPropertyChanged("LoudnessSustain");
    }
}

void Patch::LoudnessRelease::set(double value)
{
    if (loudnessRelease != value)
    {
        loudnessRelease = value;
        OnPropertyChanged("LoudnessRelease");
    }
}

void Patch::Save(String^ container)
{
    ApplicationData^ appData = ApplicationData::Current; 
    ApplicationDataContainer^ appContainer = appData->LocalSettings;

    if (container != nullptr)
    {
        appContainer = appContainer->CreateContainer(container, ApplicationDataCreateDisposition::Always);
    }

    IPropertySet^ appSettings = appContainer->Values;

    appSettings->Clear();
    appSettings->Insert("Oscillator1Waveform", this->Oscillator1Waveform);
    appSettings->Insert("Oscillator2Waveform", this->Oscillator2Waveform);

    appSettings->Insert("Oscillator1Transpose", this->Oscillator1Transpose);
    appSettings->Insert("Oscillator2Transpose", this->Oscillator2Transpose);
    appSettings->Insert("Oscillator2FineTune", this->Oscillator2FineTune);

    appSettings->Insert("Oscillator1Volume", this->Oscillator1Volume);
    appSettings->Insert("Oscillator2Volume", this->Oscillator2Volume);

    appSettings->Insert("FilterOn", this->FilterOn);
    appSettings->Insert("FilterCutoffFrequency", this->FilterCutoffFrequency);
    appSettings->Insert("FilterEmphasis", this->FilterEmphasis);
    appSettings->Insert("FilterEnvelope", this->FilterEnvelope);

    appSettings->Insert("FilterAttack", this->FilterAttack);
    appSettings->Insert("FilterDecay", this->FilterDecay);
    appSettings->Insert("FilterSustain", this->FilterSustain);
    appSettings->Insert("FilterRelease", this->FilterRelease);

    appSettings->Insert("LoudnessAttack", this->LoudnessAttack);
    appSettings->Insert("LoudnessDecay", this->LoudnessDecay);
    appSettings->Insert("LoudnessSustain", this->LoudnessSustain);
    appSettings->Insert("LoudnessRelease", this->LoudnessRelease);
}

void Patch::Load(String^ container)
{	
		ApplicationData^ appData = ApplicationData::Current;
		ApplicationDataContainer^ appContainer = appData->LocalSettings;

		if (container != nullptr &&
			appContainer->Containers->HasKey(container))
		{
			appContainer = appContainer->Containers->Lookup(container);
		}

		IPropertySet^ appSettings = appContainer->Values;

		this->Oscillator1Waveform = LoadValue(appSettings, "Oscillator1Waveform", (int)WaveformType::Square);
		this->Oscillator2Waveform = LoadValue(appSettings, "Oscillator2Waveform", (int)WaveformType::Triangle);

		this->Oscillator1Transpose = LoadValue(appSettings, "Oscillator1Transpose", 0);
		this->Oscillator2Transpose = LoadValue(appSettings, "Oscillator2Transpose", 0);
		this->Oscillator2FineTune = LoadValue(appSettings, "Oscillator2FineTune", 0.0);

		this->Oscillator1Volume = LoadValue(appSettings, "Oscillator1Volume", 100.0);        // out of 100
		this->Oscillator2Volume = LoadValue(appSettings, "Oscillator2Volume", 0);        // out of 100

		this->FilterOn = LoadValue(appSettings, "FilterOn", false);
		this->FilterCutoffFrequency = LoadValue(appSettings, "FilterCutoffFrequency", 1.0); // 1 octave
		this->FilterEmphasis = LoadValue(appSettings, "FilterEmphasis", 8.0);               // Q
		this->FilterEnvelope = LoadValue(appSettings, "FilterEnvelope", 50.0);

		this->FilterAttack = LoadValue(appSettings, "FilterAttack", 2.0);                   // 10**2 msec
		this->FilterDecay = LoadValue(appSettings, "FilterDecay", 2.0);                     // 100 msec
		this->FilterSustain = LoadValue(appSettings, "FilterSustain", 50.0);                // half max
		this->FilterRelease = LoadValue(appSettings, "FilterRelease", 3.0);                 // 1 sec

		this->LoudnessAttack = LoadValue(appSettings, "LoudnessAttack", 0.0);               // 10**2 msec
		this->LoudnessDecay = LoadValue(appSettings, "LoudnessDecay", 0.0);                 // 100 msec
		this->LoudnessSustain = LoadValue(appSettings, "LoudnessSustain", 50.0);            // half max
		this->LoudnessRelease = LoadValue(appSettings, "LoudnessRelease", 0.0);             // 1 sec
	
}

int Patch::LoadValue(IPropertySet^ appSettings, String^ key, int default)
{
    if (appSettings->HasKey(key))
        return (int)appSettings->Lookup(key);

    return default;
}

double Patch::LoadValue(IPropertySet^ appSettings, String^ key, double default)
{
    if (appSettings->HasKey(key))
        return (double)appSettings->Lookup(key);

    return default;
}

bool Patch::LoadValue(IPropertySet^ appSettings, String^ key, bool default)
{
    if (appSettings->HasKey(key))
        return (bool)appSettings->Lookup(key);

    return default;
}
