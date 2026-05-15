namespace Lyt.VideoCapture.Devices.MediaFoundation;

using global::MediaFoundation;
using global::MediaFoundation.Alt;
using global::MediaFoundation.Misc;
using global::MediaFoundation.ReadWrite;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CS8604 // Possible null reference argument.

public sealed class MfDevice : CaptureDevice, IMFSourceReaderCallback
{
    private const int MF_SOURCE_READER_FIRST_VIDEO_STREAM = unchecked((int)0xfffffffc);

    private IMFSourceReader? sourceReader;

    private IMFSourceReaderAsync? sourceReaderAsync;

    private TranscodeFormats transcodeFormat;

    private FrameProcessor? frameProcessor;

    private MfDeviceMode? mode;

    private bool firstSample;

    private long baseTime;

    private long frameIndex;

    private IntPtr pBih;
    
    private readonly object lockObject = new();

    internal string SymbolicName { get; set; } = string.Empty;

    internal string FriendlyName { get; set; } = string.Empty;

    internal int Index { get; set; }

    internal List<MfDeviceMode> SupportedModes { get; private set; } = new(32);

    internal MfDevice(object identity, string name) : base(identity, name) { }

    protected override void OnCapture(nint pData, int size, long timestampMicroseconds, long frameIndex, PixelBuffer buffer)
    {
        buffer.CopyIn(this.pBih, pData, size, timestampMicroseconds, frameIndex, this.transcodeFormat);
    }

    protected override Task OnInitializeAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        FrameProcessor frameProcessor,
        CancellationToken ct)
    {
        this.transcodeFormat = transcodeFormat;
        this.frameProcessor = frameProcessor;
        this.mode = this.SupportedModes.FirstOrDefault(m => m.IsMatching(characteristics));
        return Task.CompletedTask;
    }

    protected override Task OnStartAsync(CancellationToken ct)
    {
        // This does not work : 
        //  Thread thread = Thread.CurrentThread; 
        //  thread.SetApartmentState(System.Threading.ApartmentState.MTA); 
        // Instead use Task.Run to start the capture on a new thread, which will be MTA by default.
        Task.Run(() =>
        {
            this.StartCapture();
        });

        return Task.CompletedTask;
    }

    protected override Task OnStopAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    internal HResult StartCapture()
    {
        _ = NativeMethods.CoInitialize((IntPtr)null);
        HResult hr = MF.Startup();
        hr.ThrowExceptionOnError();

        // Create the media source for the device.
        hr = this.GetVideoSourceFromDevice(this, out IMFMediaSource? mediaSource, out IMFSourceReader? sourceReader);
        hr.ThrowExceptionOnError();
        if (mediaSource is null || sourceReader is null)
        {
            hr = HResult.E_FAIL;
            hr.ThrowExceptionOnError();
            return HResult.E_FAIL;
        }

        this.sourceReader = sourceReader;
        IMFSourceReaderAsync sourceReaderAsync = (IMFSourceReaderAsync)sourceReader;
        this.sourceReaderAsync = sourceReaderAsync;

        // Configure the reader with the user selected media type
        if (this.mode is null)
        {
            hr = HResult.E_FAIL;
            hr.ThrowExceptionOnError();
            return HResult.E_FAIL;
        }

        lock (this.lockObject)
        {
            hr = this.mode.GetMediaType(sourceReader, out IMFMediaType? mediaType);
            hr.ThrowExceptionOnError();

            hr = this.sourceReaderAsync.SetCurrentMediaType(MF_SOURCE_READER_FIRST_VIDEO_STREAM, null, mediaType);
            hr.ThrowExceptionOnError();

            hr = this.sourceReaderAsync.SetStreamSelection(MF_SOURCE_READER_FIRST_VIDEO_STREAM, true);
            hr.ThrowExceptionOnError();

            // Request the first video frame.
            hr = this.sourceReaderAsync.ReadSample(
                    MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0,
                    // actual, flags, timestamp, sample
                    IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            hr.ThrowExceptionOnError();

            this.firstSample = true;
            this.baseTime = 0;

            Marshal.ReleaseComObject(mediaSource);
            return hr;
        }
    }

    // Interface method for IMFSourceReaderCallback, must be public,
    // DO NOT throw 'Not Implemented', Returning OK is the actual implementation 
    public HResult OnFlush(int dwStreamIndex) => HResult.S_OK;

    // Interface method for IMFSourceReaderCallback, must be public,
    // DO NOT throw 'Not Implemented', Returning OK is the actual implementation 
    public HResult OnEvent(int dwStreamIndex, IMFMediaEvent pEvent) => HResult.S_OK;

    // Interface method for IMFSourceReaderCallback, must be public,
    // will be called when the source reader has a new sample or an event.
    public HResult OnReadSample(
        HResult hrStatus, int streamIndex, MF_SOURCE_READER_FLAG streamFlags, long timestamp, IMFSample sample)
    {
        //if (!this.IsCapturing)
        //{
        //    return HResult.S_OK;
        //}        

        if ((this.mode is null) || (this.sourceReaderAsync is null) || (this.frameProcessor is null))
        {
            return HResult.E_FAIL;
        }

        void NotifyFailure(Exception e)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine("OnReadSample: " + e.ToString());
            }

            // this.capturedFrames?.Add(new CapturedFrame(CapturedFrame.Kind.Error, null, e));
        }

        if (hrStatus.Failed())
        {
            // Notify app with delegate or event 
            NotifyFailure(new MFException(hrStatus));
            return hrStatus;
        }

        //int width = this.mode.Width;
        //int height = this.mode.Height;
        HResult hr = HResult.S_OK;
        IMFMediaType? mediaType = null;
        IMFMediaBuffer? mediaBuffer = null;
        try
        {
            lock (this.lockObject)
            {
                if (sample is not null)
                {
                    if (this.firstSample)
                    {
                        baseTime = timestamp;
                        this.firstSample = false;
                    }

                    // rebase the time stamp
                    timestamp -= baseTime;
                    hr = sample.SetSampleTime(timestamp);
                    hr.ThrowExceptionOnError();
                    hr = this.sourceReaderAsync.GetCurrentMediaType(0, out mediaType);
                    hr.ThrowExceptionOnError();
                    hr = MF.CreateMediaBufferFromMediaType(mediaType, timestamp, 0, 0, out mediaBuffer);
                    hr.ThrowExceptionOnError();
                    hr = sample.CopyToBuffer(mediaBuffer);
                    hr.ThrowExceptionOnError();
                    hr = mediaBuffer.Lock(out IntPtr bufferPointer, out int maxLength, out int currentLength);
                    hr.ThrowExceptionOnError();

                    nint pBuffer = bufferPointer;
                    this.frameProcessor.OnFrameArrived(
                        this, pBuffer, currentLength,
                        (long)(timestamp * 1_000_000),
                        this.frameIndex);

                    ++this.frameIndex;

                    //var rawFrame = this.frameRecycler.Get(width, height, this.BytesPerPixel);
                    //rawFrame.CaptureUtcTime = DateTime.UtcNow;
                    //if (true) //(rawFrame.Data.Length == currentLength)
                    //{
                    //    Marshal.Copy(bufferPointer, rawFrame.Data, 0, currentLength);
                    //    this.capturedFrames.Add(new CapturedFrame(CapturedFrame.Kind.Sample, rawFrame));
                    //}
                    //else
                    //{
                    //    this.capturedFrames.Add(new CapturedFrame(CapturedFrame.Kind.Error));
                    //}

                    hr = mediaBuffer.Unlock();
                    hr.ThrowExceptionOnError();
                }
                else
                {
                    Debug.WriteLine("Empty");
                    // this.capturedFrames.Add(new CapturedFrame(CapturedFrame.Kind.Empty));
                }

                // Request next sample 
                // IMPORTANT => This requires MTA thread, otherwise it will fail with No Such Interface .
                // This must be called to continue receiving samples, even if the current sample is null or
                // in case of an error.
                hr = this.sourceReaderAsync.ReadSample(
                    MF_SOURCE_READER_FIRST_VIDEO_STREAM, 0,
                    // actual, flags, timestamp, sample, dont care in Async mode
                    IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                if (hr.Failed())
                {
                    Debug.WriteLine("Next Sample Failed");
                    //this.capturedFrames.Add(new CapturedFrame(CapturedFrame.Kind.Error, null, new MFException(hr)));
                }
            }
        }
        catch (MFException e)
        {
            Debug.WriteLine("OnReadSample: " + e.ToString());
            NotifyFailure(e);
        }
        catch (Exception e)
        {
            hr = (HResult)Marshal.GetHRForException(e);
            Debug.WriteLine("OnReadSample: " + " hr: " + hrStatus.ToString() + e.ToString());
            NotifyFailure(e);
        }
        finally
        {
            // Always release our local COM objects 
            if (mediaType is not null)
            {
                Marshal.ReleaseComObject(mediaType);
            }

            if (mediaBuffer is not null)
            {
                Marshal.ReleaseComObject(mediaBuffer);
            }

            if (sample is not null)
            {
                // Always release the sample, if not null 
                Marshal.ReleaseComObject(sample);
            }
        }

        return hr;
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

    internal HResult GetVideoSourceFromDevice(
        IMFSourceReaderCallback sourceReaderCallback,
        out IMFMediaSource? mediaSource,
        out IMFSourceReader? sourceReader)
    {
        mediaSource = null;
        sourceReader = null;

        IMFAttributes configAttributes = MF.CreateAttributes(1);
        // From working sample: 
        // MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID, 0x8ac3587a, 0x4ae7, 0x42d8, 0x99, 0xe0, 0x0a, 0x60, 0x13, 0xee, 0xf9, 0x0f);
        Guid MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID = new("8ac3587a-4ae7-42d8-99e0-0a6013eef90f");
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
