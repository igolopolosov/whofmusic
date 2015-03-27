#pragma once
#include "AmplitudeEnvelopeParameters.h"
#include "EnvelopeGenerator.h"

class AmplitudeEnvelopeEffect sealed : public CXAPOParametersBase
{
private:
    static const XAPO_REGISTRATION_PROPERTIES RegistrationProps;
    byte * pParameterBlocks;
    WAVEFORMATEX waveFormat;
    EnvelopeGenerator envelopeGenerator;
    bool keyPressed;

public:
    static AmplitudeEnvelopeEffect * Create();
    ~AmplitudeEnvelopeEffect();

protected:
    AmplitudeEnvelopeEffect(const XAPO_REGISTRATION_PROPERTIES * pRegProperties, 
                            BYTE * pParameterBlocks,
                            UINT32 uParameterBlockByteSize,
                            BOOL fProducer);

    virtual HRESULT __stdcall LockForProcess(UINT32 inpParamCount,
                                             const XAPO_LOCKFORPROCESS_BUFFER_PARAMETERS  *pInpParams,
                                             UINT32 outParamCount,
                                             const XAPO_LOCKFORPROCESS_BUFFER_PARAMETERS  *pOutParam) override;

    virtual void __stdcall Process(UINT32 inpParameterCount,
                                   const XAPO_PROCESS_BUFFER_PARAMETERS *pInpParams,
                                   UINT32 outParameterCount,
                                   XAPO_PROCESS_BUFFER_PARAMETERS *pOutParams,
                                   BOOL isEnabled) override;
};

class __declspec(uuid("8A06B1AC-A164-4F2D-979C-C54BE66DF803")) AmplitudeEnvelopeEffect;
