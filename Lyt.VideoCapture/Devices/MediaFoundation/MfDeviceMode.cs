namespace Lyt.VideoCapture.Devices.MediaFoundation;

using global::MediaFoundation;
using global::MediaFoundation.Alt;
using global::MediaFoundation.Misc;
using global::MediaFoundation.ReadWrite;
using global::MediaFoundation.Transform;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
//#pragma warning disable CA1416 // Validate platform compatibility
//#pragma warning disable CS8604 // Possible null reference argument.

internal sealed class MfDeviceMode 
{
    public MfDeviceMode (IMFMediaType mediaType)
    {
        HResult hr = mediaType.GetMajorType(out Guid guidMajorType);
        hr.ThrowExceptionOnError();
        this.MediaMajorType = guidMajorType;
        this.MediaMajorTypeString = guidMajorType.ToName();
        hr = mediaType.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out Guid subTypeGuid);
        hr.ThrowExceptionOnError();
        this.MediaSubType = subTypeGuid;
        this.MediaTypeString = subTypeGuid.ToName();

        hr = mediaType.GetCount(out int attributeCount);
        hr.ThrowExceptionOnError();
        for (int i = 0; i < attributeCount; ++i)
        {
            hr = mediaType.GetItemByIndex(i, out Guid guid, null);
            hr.ThrowExceptionOnError();
            this.ModeId = guid;

            hr = mediaType.GetItemType(guid, out MFAttributeType attrType);
            hr.ThrowExceptionOnError();

            switch (attrType)
            {
                case MFAttributeType.Uint64:
                    hr = mediaType.GetUINT64(guid, out ulong value);
                    hr.ThrowExceptionOnError();
                    int high = value.High32();
                    int low = value.Low32();
                    if (guid == MFAttributesClsid.MF_MT_FRAME_SIZE)
                    {
                        this.Width = high;
                        this.Height = low;
                    }
                    else if (guid == MFAttributesClsid.MF_MT_FRAME_RATE)
                    {
                        // Frame rate is numerator / denominator.
                        this.FrameRateNumerator = high;
                        this.FrameRateDenominator = low;
                    }
                    else if (guid == MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO)
                    {
                        // AS is also rate is numerator / denominator.
                        this.AspectRatioNumerator = high;
                        this.AspectRatioDenominator = low;
                    }
                    break;


                // We care only about uint 64 values, nothing interesting below 
                default:
                case MFAttributeType.None:
                case MFAttributeType.Blob:
                case MFAttributeType.Double:
                case MFAttributeType.Guid:
                case MFAttributeType.IUnknown:
                case MFAttributeType.String:
                case MFAttributeType.Uint32:
                    break;
            }
        }
    }

    public Guid ModeId { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int FrameRateNumerator { get; set; }

    public int FrameRateDenominator { get; set; }

    public Guid MediaMajorType { get; set; }

    public string MediaMajorTypeString { get; set; }

    public Guid MediaSubType { get; set; }

    public string MediaTypeString { get; set; }

    public int AspectRatioNumerator { get; set; }

    public int AspectRatioDenominator { get; set; }

    public float FrameRate
        => this.FrameRateDenominator != 0 ? this.FrameRateNumerator / (float)this.FrameRateDenominator : 0.0f;

    public float AspectRatio
        => this.AspectRatioDenominator != 0 ? this.AspectRatioNumerator / (float)this.AspectRatioDenominator : 0.0f;

    internal VideoCharacteristics ToVideoCharacteristics()
        => new(
            this.PixelFormat, 
            this.Width, 
            this.Height,
            new Fraction(this.FrameRateNumerator, this.FrameRateDenominator),
            this.ToString(), 
            true, 
            this.MediaTypeString);

    internal bool IsMatching (VideoCharacteristics characteristics)
        => this.PixelFormat == characteristics.PixelFormat &&
           this.Width == characteristics.Width &&
           this.Height == characteristics.Height &&
           this.FrameRateNumerator == characteristics.FramesPerSecond.Numerator &&
           this.FrameRateDenominator == characteristics.FramesPerSecond.Denominator;
    
    internal PixelFormats PixelFormat =>
        this.MediaSubType switch
        {
            _ when this.MediaSubType == MFMediaType.ARGB32 => PixelFormats.ARGB32,
            _ when this.MediaSubType == MFMediaType.RGB8 => PixelFormats.RGB8,
            _ when this.MediaSubType == MFMediaType.RGB24 => PixelFormats.RGB24,
            _ when this.MediaSubType == MFMediaType.RGB32 => PixelFormats.RGB32,
            _ when this.MediaSubType == MFMediaType.ARGB32 => PixelFormats.ARGB32,
            _ when this.MediaSubType == MFMediaType.MJPG => PixelFormats.JPEG,
            _ when this.MediaSubType == MFMediaType.UYVY => PixelFormats.UYVY,
            _ when this.MediaSubType == MFMediaType.NV12 => PixelFormats.NV12,

            // Could be wrong ! 
            _ when this.MediaSubType == MFMediaType.YVYU => PixelFormats.YUYV,

            // Apparently not available in Media Foundation,
            // _ when this.MediaSubType == MFMediaType.PNG => PixelFormats.PNG,

            _ => PixelFormats.Unknown,
        };

    public override string ToString()
        => string.Format(
            "{0} ~ {1}  -  {2}x{3}  ({4}:{5})  at {6} fps.",
            this.MediaMajorTypeString, this.MediaTypeString,
            this.Width, this.Height, this.AspectRatioNumerator, this.AspectRatioDenominator,
            this.FrameRate);

    public bool IsMatching(IMFMediaType mediaType)
    {
        MfDeviceMode other = new(mediaType);
        return
            this.Width == other.Width &&
            this.Height == other.Height &&
            this.MediaMajorType == other.MediaMajorType &&
            this.MediaSubType == other.MediaSubType &&
            this.AspectRatioDenominator == other.AspectRatioDenominator &&
            this.AspectRatioNumerator == other.AspectRatioNumerator &&
            this.FrameRateDenominator == other.FrameRateDenominator &&
            this.FrameRateNumerator == other.FrameRateNumerator;
    }

    public HResult GetMediaType(IMFSourceReader sourceReader, out IMFMediaType? selectedMediaType)
    {
        selectedMediaType = null;
        int mediaTypeIndex = 0;
        HResult hr = HResult.S_OK;
        while (hr.Succeeded())
        {
            hr = sourceReader.GetNativeMediaType(0, mediaTypeIndex, out IMFMediaType mediaType);
            if (hr == HResult.MF_E_NO_MORE_TYPES)
            {
                return HResult.E_FAIL;
            }
            else if (hr.Succeeded() && (mediaType is not null))
            {
                if (this.IsMatching(mediaType) )
                {
                    selectedMediaType = mediaType;
                    return HResult.S_OK;
                }
            }

            ++mediaTypeIndex;
        }

        return HResult.E_FAIL;
    }
}

#pragma warning restore CA8625 
