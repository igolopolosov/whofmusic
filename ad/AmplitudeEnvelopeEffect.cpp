#include "pch.h"
#include "AmplitudeEnvelopeEffect.h"

const XAPO_REGISTRATION_PROPERTIES AmplitudeEnvelopeEffect::RegistrationProps = 
{
    __uuidof(AmplitudeEnvelopeEffect),
    L"Amplitude Envelope Effect",
    L"Coded by Charles Petzold",
    1,      // major version number
    0,      // minor version number
    XAPOBASE_DEFAULT_FLAG | XAPO_FLAG_INPLACE_REQUIRED,
    1,      // min input buffer count
    1,      // max input buffer count
    1,      // min output buffer count
    1       // max output buffer count
};

AmplitudeEnvelopeEffect * AmplitudeEnvelopeEffect::Create()
{
    // Create and initialize three effect parameters
    AmplitudeEnvelopeParameters * pParameterBlocks = new AmplitudeEnvelopeParameters[3];

    for (int i = 0; i < 3; i++)
    {
        pParameterBlocks[i].keyPressed = false;
        pParameterBlocks[i].envelopeParams.baseLevel = 0;
    }

    // Create the effect
    return new AmplitudeEnvelopeEffect(&RegistrationProps, 
                                       (byte *) pParameterBlocks, 
                                       sizeof(AmplitudeEnvelopeParameters), 
                                       false);
}

AmplitudeEnvelopeEffect::AmplitudeEnvelopeEffect(const XAPO_REGISTRATION_PROPERTIES * pRegProperties, 
                                                 BYTE * pParameterBlocks,
                                                 UINT32 uParameterBlockByteSize,
                                                 BOOL fProducer) 
                                                 :
                                                 CXAPOParametersBase(pRegProperties, 
                                                                     pParameterBlocks, 
                                                                     uParameterBlockByteSize, 
                                                                     fProducer),
                                                 keyPressed(false)
{
    // Save parameters blocks for deletion in destructor
    this->pParameterBlocks = pParameterBlocks;
}

AmplitudeEnvelopeEffect::~AmplitudeEnvelopeEffect()
{
    if (pParameterBlocks != nullptr)
        delete[] pParameterBlocks;
}

HRESULT AmplitudeEnvelopeEffect::LockForProcess(UINT32 inpParamCount,
                                                const XAPO_LOCKFORPROCESS_BUFFER_PARAMETERS  *pInpParams,
                                                UINT32 outParamCount,
                                                const XAPO_LOCKFORPROCESS_BUFFER_PARAMETERS  *pOutParams)
{
    waveFormat = * pInpParams[0].pFormat;
    return CXAPOBase::LockForProcess(inpParamCount, pInpParams, outParamCount, pOutParams);
}

void AmplitudeEnvelopeEffect::Process(UINT32 inpParamCount,
                                      const XAPO_PROCESS_BUFFER_PARAMETERS *pInpParam,
                                      UINT32 outParamCount,
                                      XAPO_PROCESS_BUFFER_PARAMETERS *pOutParam,
                                      BOOL isEnabled)
{
    // Get effect parameters
    AmplitudeEnvelopeParameters * pParams = 
        reinterpret_cast<AmplitudeEnvelopeParameters *>(CXAPOParametersBase::BeginProcess());

    // Get buffer pointers and other information
    const float * pSrc = static_cast<float const*>(pInpParam[0].pBuffer);
    float * pDst = static_cast<float *>(pOutParam[0].pBuffer);
    int frameCount = pInpParam[0].ValidFrameCount;
    int numChannels = waveFormat.nChannels;

    switch(pInpParam[0].BufferFlags)
    {
    case XAPO_BUFFER_VALID:
        if (!isEnabled)
        {
            for (int frame = 0; frame < frameCount; frame++)
            {
                for (int channel = 0; channel < numChannels; channel++)
                {
                    int index = numChannels * frame + channel;
                    pDst[index] = pSrc[index];
                }
            }
        }
        else
        {
            // Key being pressed
            if (!this->keyPressed && pParams->keyPressed)
            {
                this->keyPressed = true;
                this->envelopeGenerator.SetParameters(pParams->envelopeParams);
                this->envelopeGenerator.Attack();
            }
            // Key being released
            else if (this->keyPressed && !pParams->keyPressed)
            {
                this->keyPressed = false;
                this->envelopeGenerator.Release();
            }

            // Calculate interval in msec
            float interval = 1000.0f / waveFormat.nSamplesPerSec;

            for (int frame = 0; frame < frameCount; frame++)
            {
                float volume;
                envelopeGenerator.GetNextValue(interval, volume);

                for (int channel = 0; channel < numChannels; channel++)
                {
                    int index = numChannels * frame + channel;
                    pDst[index] = volume * pSrc[index];
                }
            }
        }
        break;

    case XAPO_BUFFER_SILENT:
        break;
    }

    // Set output parameters
    pOutParam[0].ValidFrameCount = pInpParam[0].ValidFrameCount;
    pOutParam[0].BufferFlags = pInpParam[0].BufferFlags;

    CXAPOParametersBase::EndProcess();
}
