#pragma once

#include <pch.h>

// Most important components 
#include "SynthVoice.h"
#include "Patch.h"

// Plain old data structures
#include "DynamicSynthVoiceParameters.h"
#include "DynamicFilterParameters.h"
#include "KeyPressedArgs.h"

namespace WomAudioComponent
{
	public ref class AudioController sealed
	{
	private:
		~AudioController();

		Microsoft::WRL::ComPtr<IXAudio2> pXAudio2;
		std::vector<SynthVoice *> allSynthVoices;
		std::vector<SynthVoice *> availableSynthVoices;
		std::map<int, SynthVoice *> playingSynthVoices;
		Patch^ patch;
		IXAudio2MasteringVoice * pMasteringVoice;

		DynamicSynthVoiceParameters dynamicSynthVoiceParameters;

		void OnDynamicPropertyChanged(Platform::Object^ sender, int args);
		void OnFilterEffectPropertyChanged(Platform::Object^ sender, int args);

	public:
		AudioController();
		void CreatePatch();
		void ReleaseVoices();
		Patch^ GetPatch();
		bool IsOverloadPeaks();
		void SetVolumeLevel(int volumeLevel);
		void Start();		
		void Stop();
		void KeyIsPressedChanged(Platform::Object^ sender, KeyPressedArgs^ args);
	};
}