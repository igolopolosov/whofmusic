#pragma once
#include "FilterEnvelopeParameters.h"
#include "EnvelopeGenerator.h"

class FilterEnvelopeEffect sealed : public CXAPOParametersBase
{
private:
    static const XAPO_REGISTRATION_PROPERTIES RegistrationProps;
    byte * pParameterBlocks;

    WAVEFORMATEX waveFormat;
    EnvelopeGenerator envelopeGenerator;
    bool keyPressed;
    float * pxp, * pxpp, * pyp, * pypp;

public:
    static FilterEnvelopeEffect * Create();
    ~FilterEnvelopeEffect();

private:
    void FreeArrays();

protected:
    FilterEnvelopeEffect(const XAPO_REGISTRATION_PROPERTIES * pRegProperties, 
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

class __declspec(uuid("74C23373-C5AA-4AB6-BD51-AF12D37EB41C")) FilterEnvelopeEffect;
