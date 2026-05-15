namespace Lyt.VideoCapture.Devices.MediaFoundation;

using global::MediaFoundation;
using global::MediaFoundation.ReadWrite;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CS8604 // Possible null reference argument.

public sealed class MfDevice : CaptureDevice
{
    internal string SymbolicName { get; set; } = string.Empty;

    internal string FriendlyName { get; set; } = string.Empty;

    internal int Index { get; set; } 

    internal List<MfDeviceMode> SupportedModes { get; private set; } = new(32);

    internal MfDevice(object identity, string name) : base(identity, name) { }

    protected override void OnCapture(nint pData, int size, long timestampMicroseconds, long frameIndex, PixelBuffer buffer)
    {

    }

    protected override Task OnInitializeAsync(VideoCharacteristics characteristics, TranscodeFormats transcodeFormat, FrameProcessor frameProcessor, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    protected override Task OnStartAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    protected override Task OnStopAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    internal HResult EnumerateModes(IMFSourceReader sourceReader)
    {
        int mediaTypeIndex = 0;
        HResult hr = HResult.S_OK;
        while (hr.Succeeded())
        {
            hr = sourceReader.GetNativeMediaType(0, mediaTypeIndex, out IMFMediaType mediaType);
            if (hr == HResult.MF_E_NO_MORE_TYPES)
            {
                return HResult.S_OK;
            }
            else if (hr.Succeeded() && (mediaType is not null))
            {
                var mode = new MfDeviceMode(mediaType);
                Debug.WriteLine(mode);
                this.SupportedModes.Add(mode);
#pragma warning disable CA1416 
                // Validate platform compatibility
                Marshal.ReleaseComObject(mediaType);
            }

            ++mediaTypeIndex;
        }

        return HResult.S_OK;
    }
    public HResult GetVideoSourceFromDevice(
        IMFSourceReaderCallback sourceReaderCallback,
        out IMFMediaSource? mediaSource, 
        out IMFSourceReader? sourceReader)
    {
        mediaSource = null;
        sourceReader = null;

        IMFAttributes configAttributes = MF.CreateAttributes(1);
        // From working sample: 
        // MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID, 0x8ac3587a, 0x4ae7, 0x42d8, 0x99, 0xe0, 0x0a, 0x60, 0x13, 0xee, 0xf9, 0x0f);
        Guid MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID = new ("8ac3587a-4ae7-42d8-99e0-0a6013eef90f");
        HResult hr = configAttributes.SetGUID(
            MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
            MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
        hr.ThrowExceptionOnError();
        hr = MF.EnumDeviceSources(configAttributes, out IMFActivate[] sourceActivate);
        hr.ThrowExceptionOnError();
        Debug.WriteLine("Device(s) Found: " + sourceActivate.Length.ToString());

        if ((this.Index < 0) || (this.Index >= sourceActivate.Length))
        {
            Debug.WriteLine("Device Index out of range: " + this.Index.ToString() + " " + sourceActivate.Length.ToString());
            return HResult.E_INVALIDARG;
        }

        var source = sourceActivate[this.Index];

        // Names should match as well 
        hr = source.GetAllocatedString(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME, out string friendlyName);
        hr.ThrowExceptionOnError();
        if (friendlyName != this.FriendlyName)
        {
            Debug.WriteLine("Device Name Invalid: " + friendlyName);
            return HResult.E_INVALIDARG;
        }

        Guid riidMediaSource = Helper.GetGuid<IMFMediaSource>();
        hr = source.ActivateObject(riidMediaSource, out object ppv);
        hr.ThrowExceptionOnError();
        if (ppv is not IMFMediaSource)
        {
            hr.ThrowExceptionOnError();
        }

        mediaSource = ppv as IMFMediaSource;

        // Doc link for more attributes (incl. latency) 
        IMFAttributes attributes = MF.CreateAttributes(2);
        hr = attributes.SetUINT32(MFAttributesClsid.MF_SOURCE_READER_ENABLE_VIDEO_PROCESSING, 1);
        hr.ThrowExceptionOnError();
        hr = attributes.SetUnknown(MFAttributesClsid.MF_SOURCE_READER_ASYNC_CALLBACK, sourceReaderCallback);
        hr.ThrowExceptionOnError();
        hr = MF.CreateSourceReaderFromMediaSource(mediaSource, attributes, out sourceReader);
#pragma warning restore CS8604 // Possible null reference argument.
        hr.ThrowExceptionOnError();

        // Final cleanup 
        // No release of sourceActivate (Don't try!) 
        Marshal.ReleaseComObject(configAttributes);
        Marshal.ReleaseComObject(attributes);

        return HResult.S_OK;
    }
}

#pragma warning restore CA1416 
#pragma warning restore CA8604
#pragma warning restore CA8625 
