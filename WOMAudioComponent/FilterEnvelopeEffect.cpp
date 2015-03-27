#include "pch.h"
#include "FilterEnvelopeEffect.h"

using namespace DirectX;

const XAPO_REGISTRATION_PROPERTIES FilterEnvelopeEffect::RegistrationProps = 
{
    __uuidof(FilterEnvelopeEffect),
    L"Filter Envelope Effect",
    L"Coded by Charles Petzold",
    1,      // major version number
    0,      // minor version number
    XAPOBASE_DEFAULT_FLAG | XAPO_FLAG_INPLACE_REQUIRED,
    1,      // min input buffer count
    1,      // max input buffer count
    1,      // min output buffer count
    1       // max output buffer count
};

FilterEnvelopeEffect * FilterEnvelopeEffect::Create()
{
    // Create and initialize three effect parameters
    FilterEnvelopeParameters * pParameterBlocks = new FilterEnvelopeParameters[3];

    for (int i = 0; i < 3; i++)
    {
        pParameterBlocks[i].keyPressed = false;
        pParameterBlocks[i].envelopeParams.baseLevel = 1;
    }
    
    // Create the effect
    return new FilterEnvelopeEffect(&RegistrationProps, 
                                    (byte *) pParameterBlocks, 
                                    sizeof(FilterEnvelopeParameters), 
                                    false);
}

FilterEnvelopeEffect::FilterEnvelopeEffect(const XAPO_REGISTRATION_PROPERTIES * pRegProperties, 
                                           BYTE * pParameterBlocks,
                                           UINT32 uParameterBlockByteSize,
                                           BOOL fProducer) 
                                           :
                                           CXAPOParametersBase(pRegProperties, 
                                                               pParameterBlocks, 
                                                               uParameterBlockByteSize, 
                                                               fProducer),
                                           keyPressed(false),
                                           pxp(nullptr),
                                           pxpp(nullptr),
                                           pyp(nullptr),
                                           pypp(nullptr)
{
    // Save parameters blocks for deletion in destructor
    this->pParameterBlocks = pParameterBlocks;
}

FilterEnvelopeEffect::~FilterEnvelopeEffect()
{
    FreeArrays();

    if (pParameterBlocks != nullptr)
        delete[] pParameterBlocks;
}

void FilterEnvelopeEffect::FreeArrays()
{
    if (pxp != nullptr)
        delete[] pxp;

    if (pxpp != nullptr)
        delete[] pxpp;

    if (pyp != nullptr)
        delete[] pyp;

    if (pypp != nullptr)
        delete[] pypp;
}

HRESULT FilterEnvelopeEffect::LockForProcess(UINT32 inpParamCount,
                                             const XAPO_LOCKFORPROCESS_BUFFER_PARAMETERS  *pInpParams,
                                             UINT32 outParamCount,
                                             const XAPO_LOCKFORPROCESS_BUFFER_PARAMETERS  *pOutParams)
{
    waveFormat = * pInpParams[0].pFormat;

    // Verify sufficiently high sample rate
    if (waveFormat.nSamplesPerSec < 44100)
        return E_NOTIMPL;

    FreeArrays();

    // Allocate memory for saving previous samples
    int channels = waveFormat.nChannels;

    pxp = new float[channels];
    pxpp = new float[channels];
    pyp = new float[channels];
    pypp = new float[channels];

    for (int channel = 0; channel < channels; channel++)
    {
        pxp[channel] = 0.0f;
        pxpp[channel] = 0.0f;
        pyp[channel] = 0.0f;
        pypp[channel] = 0.0f;
    }

    return CXAPOBase::LockForProcess(inpParamCount, pInpParams, outParamCount, pOutParams);
}

void FilterEnvelopeEffect::Process(UINT32 inpParamCount,
                                   const XAPO_PROCESS_BUFFER_PARAMETERS *pInpParam,
                                   UINT32 outParamCount,
                                   XAPO_PROCESS_BUFFER_PARAMETERS *pOutParam,
                                   BOOL isEnabled)
{
    // Get effect parameters
    FilterEnvelopeParameters * pParams = 
        reinterpret_cast<FilterEnvelopeParameters *>(CXAPOParametersBase::BeginProcess());

    // Get buffer pointers and other information
    const float * pSrc = static_cast<float const *>(pInpParam[0].pBuffer);
    float * pDst = static_cast<float *>(pOutParam[0].pBuffer);
    int frameCount = pInpParam[0].ValidFrameCount;
    int numChannels = waveFormat.nChannels;

    switch(pInpParam[0].BufferFlags)
    {
    case XAPO_BUFFER_VALID:
        if (!isEnabled)
        {
            for (int frame = 0; frame < frameCount; frame++)
                for (int channel = 0; channel < numChannels; channel++)
                {
                    int index = numChannels * frame + channel;
                    pDst[index] = pSrc[index];
                }
        }
        else
        {
            float cutoffMultiplier = 1;

            // Key being pressed
            if (pParams->isTriggered && !this->keyPressed && pParams->keyPressed)
            {
                // Clear out the saved samples
                for (int channel = 0; channel < numChannels; channel++)
                {
                    pxp[channel] = 0.0f;
                    pxpp[channel] = 0.0f;
                    pyp[channel] = 0.0f;
                    pypp[channel] = 0.0f;
                }

                this->keyPressed = true;
                this->envelopeGenerator.SetParameters(pParams->envelopeParams);
                this->envelopeGenerator.Attack();
            }
            else
            {
                // Key being released
                if (pParams->isTriggered && this->keyPressed && !pParams->keyPressed)
                {
                    this->keyPressed = false;
                    this->envelopeGenerator.Release();
                }

                // Calculate interval in msec
                float interval = 1000.0f * frameCount / waveFormat.nSamplesPerSec;
                envelopeGenerator.GetNextValue(interval, cutoffMultiplier);
            }

            // Calculate filter factors
            float Q = pParams->filterEmphasis;
            float cutoffFrequency = pParams->filterCutoffFrequency * cutoffMultiplier;

            // Limit cutoff frequency to 1/3 sampling rate.
            cutoffFrequency = min(cutoffFrequency, waveFormat.nSamplesPerSec / 3);
            float omega = XM_2PI * cutoffFrequency / waveFormat.nSamplesPerSec;

            float sine;
            float cosine;
            XMScalarSinCos(&sine, &cosine, omega);
            float alpha = sine / (2 * Q);

            float a0 = 1 + alpha;
            float a1 = -2 * cosine;
            float a2 = 1 - alpha;
            float b0 = (1 - cosine) / 2;
            float b1 = 1 - cosine;
            float b2 = (1 - cosine) / 2;

            // Loop through the frames and channels
            for (int frame = 0; frame < frameCount; frame++)
            {
                for (int channel = 0; channel < numChannels; channel++)
                {
                    int index = numChannels * frame + channel;

                    // Get previous inputs
                    float x = pSrc[index];
                    float xp = pxp[channel];
                    float xpp = pxpp[channel];

                    // Get previous outputs
                    float yp = pyp[channel];
                    float ypp = pypp[channel];

                    // Calculate filter output
                    float y = (b0 * x + b1 * xp + b2 * xpp - a1 * yp - a2 * ypp) / a0;

                    // Store value -- not adjusted for filter gain!
                    pDst[index] = y;

                    // Save previous output values
                    pypp[channel] = yp;
                    pyp[channel] = y;

                    // Save previous input values
                    pxpp[channel] = xp;
                    pxp[channel] = x;
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
