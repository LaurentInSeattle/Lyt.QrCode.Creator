
namespace Lyt.VideoCapture.Devices; 

public delegate void PixelBufferArrivedDelegate(
    PixelBufferScope bufferScope);

public delegate Task PixelBufferArrivedTaskDelegate(
    PixelBufferScope bufferScope);

public abstract class CaptureDeviceDescriptor
{
    private readonly AsyncLock locker = new();

    internal readonly BufferPool defaultBufferPool;

    protected CaptureDeviceDescriptor(
        string name, string description,
        VideoCharacteristics[] characteristics,
        BufferPool defaultBufferPool)
    {
        this.Name = name;
        this.Description = description;
        this.Characteristics = characteristics;
        this.defaultBufferPool = defaultBufferPool;
    }

    public abstract object Identity { get; }
    public abstract DeviceTypes DeviceType { get; }
    public string Name { get; }
    public string Description { get; }
    public VideoCharacteristics[] Characteristics { get; }

    protected abstract Task<CaptureDevice> OnOpenWithFrameProcessorAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        FrameProcessor frameProcessor,
        CancellationToken ct);

    public override string ToString() =>
        $"{this.Name}: {this.Description}, Characteristics={this.Characteristics.Length}";


    public Task<CaptureDevice> OpenWithFrameProcessorAsync(      
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        FrameProcessor frameProcessor,
        CancellationToken ct = default) =>
        this.InternalOpenWithFrameProcessorAsync(characteristics, transcodeFormat, frameProcessor, ct);

    public Task<CaptureDevice> OpenAsync(        
        VideoCharacteristics characteristics,
        PixelBufferArrivedDelegate pixelBufferArrived,
        CancellationToken ct = default) =>
        this.OpenWithFrameProcessorAsync(
            characteristics, TranscodeFormats.Auto,
            new DelegatedQueuingProcessor(pixelBufferArrived, 1, this.defaultBufferPool),
            ct);

    public Task<CaptureDevice> OpenAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        PixelBufferArrivedDelegate pixelBufferArrived,
        CancellationToken ct = default) =>
        this.OpenWithFrameProcessorAsync(
            characteristics, transcodeFormat,
            new DelegatedQueuingProcessor(pixelBufferArrived, 1, this.defaultBufferPool),
            ct);

    public Task<CaptureDevice> OpenAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        bool isScattering,
        int maxQueuingFrames,
        PixelBufferArrivedDelegate pixelBufferArrived,
        CancellationToken ct = default) =>
        this.OpenWithFrameProcessorAsync(
            characteristics, transcodeFormat,
            isScattering ?
                new DelegatedScatteringProcessor(pixelBufferArrived, maxQueuingFrames, this.defaultBufferPool) :
                new DelegatedQueuingProcessor(pixelBufferArrived, maxQueuingFrames, this.defaultBufferPool),
            ct);

    //////////////////////////////////////////////////////////////////////////////////

    public Task<CaptureDevice> OpenAsync(
        VideoCharacteristics characteristics,
        PixelBufferArrivedTaskDelegate pixelBufferArrived,
        CancellationToken ct = default) =>
        this.OpenWithFrameProcessorAsync(
            characteristics, TranscodeFormats.Auto,
            new DelegatedQueuingTaskProcessor(pixelBufferArrived, 1, this.defaultBufferPool),
            ct);

    public Task<CaptureDevice> OpenAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        PixelBufferArrivedTaskDelegate pixelBufferArrived,
        CancellationToken ct = default) =>
        this.OpenWithFrameProcessorAsync(
            characteristics, transcodeFormat,
            new DelegatedQueuingTaskProcessor(pixelBufferArrived, 1, this.defaultBufferPool),
            ct);

    public Task<CaptureDevice> OpenAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        bool isScattering,
        int maxQueuingFrames,
        PixelBufferArrivedTaskDelegate pixelBufferArrived,
        CancellationToken ct = default) =>
        this.OpenWithFrameProcessorAsync(
            characteristics, transcodeFormat,
            isScattering ?
                new DelegatedScatteringTaskProcessor(pixelBufferArrived, maxQueuingFrames, this.defaultBufferPool) :
                new DelegatedQueuingTaskProcessor(pixelBufferArrived, maxQueuingFrames, this.defaultBufferPool),
            ct);

    //////////////////////////////////////////////////////////////////////////////////

    public async Task<ObservableCaptureDevice> AsObservableAsync(
        VideoCharacteristics characteristics,
        CancellationToken ct = default)
    {
        var observerProxy = new ObservableCaptureDevice.ObserverProxy();
        var captureDevice = await this.OpenWithFrameProcessorAsync(
            characteristics, TranscodeFormats.Auto,
            new DelegatedQueuingProcessor(observerProxy.OnPixelBufferArrived, 1, this.defaultBufferPool),
            ct).
            ConfigureAwait(false);

        return new ObservableCaptureDevice(captureDevice, observerProxy);
    }

    public async Task<ObservableCaptureDevice> AsObservableAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        CancellationToken ct = default)
    {
        var observerProxy = new ObservableCaptureDevice.ObserverProxy();
        var captureDevice = await this.OpenWithFrameProcessorAsync(
            characteristics, transcodeFormat,
            new DelegatedQueuingProcessor(observerProxy.OnPixelBufferArrived, 1, this.defaultBufferPool),
            ct).
            ConfigureAwait(false);

        return new ObservableCaptureDevice(captureDevice, observerProxy);
    }

    public async Task<ObservableCaptureDevice> AsObservableAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        bool isScattering,
        int maxQueuingFrames,
        CancellationToken ct = default)
    {
        var observerProxy = new ObservableCaptureDevice.ObserverProxy();
        var captureDevice = await this.OpenWithFrameProcessorAsync(
            characteristics, transcodeFormat,
            isScattering ?
                new DelegatedScatteringProcessor(observerProxy.OnPixelBufferArrived, maxQueuingFrames, this.defaultBufferPool) :
                new DelegatedQueuingProcessor(observerProxy.OnPixelBufferArrived, maxQueuingFrames, this.defaultBufferPool),
            ct).
            ConfigureAwait(false);

        return new ObservableCaptureDevice(captureDevice, observerProxy);
    }

    //////////////////////////////////////////////////////////////////////////////////

    public Task<byte[]> TakeOneShotAsync(
        VideoCharacteristics characteristics,
        CancellationToken ct = default) =>
        this.InternalTakeOneShotAsync(characteristics, TranscodeFormats.Auto, ct);

    public Task<byte[]> TakeOneShotAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        CancellationToken ct = default) =>
        this.InternalTakeOneShotAsync(characteristics, transcodeFormat, ct);

    //////////////////////////////////////////////////////////////////////////

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Task<CaptureDevice> InternalOpenWithFrameProcessorAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        FrameProcessor frameProcessor,
        CancellationToken ct) =>
        this.OnOpenWithFrameProcessorAsync(characteristics, transcodeFormat, frameProcessor, ct);

    internal async Task<CaptureDevice> InternalOnOpenWithFrameProcessorAsync(
        CaptureDevice preConstructedDevice,
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        FrameProcessor frameProcessor,
        CancellationToken ct)
    {
        if (characteristics.PixelFormat == PixelFormats.Unknown)
        {
            throw new ArgumentException(
                $"Couldn't use unknown pixel format: {characteristics} ({characteristics.RawPixelFormat})");
        }

        using var _ = await this.locker.LockAsync(ct);

        try
        {
            await preConstructedDevice.InternalInitializeAsync(
                characteristics, transcodeFormat, frameProcessor, ct);
        }
        catch
        {
            preConstructedDevice.Dispose();
            throw;
        }
        return preConstructedDevice;
    }

    internal async Task<byte[]> InternalTakeOneShotAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<byte[]>();

        using var device = await this.OnOpenWithFrameProcessorAsync(
            characteristics, transcodeFormat,
            new DelegatedQueuingProcessor(pixelBuffer =>
            {
                var image = pixelBuffer.Buffer.InternalExtractImage(
                    PixelBuffer.BufferStrategies.ForceCopy);
                Debug.Assert(image.Array!.Length == image.Count);

                pixelBuffer.InternalReleaseNow();

                tcs.TrySetResult(image.Array);
            }, 1, new DefaultBufferPool()),
            ct);

        await device.InternalStartAsync(ct);
        var image = await tcs.Task;
        await device.InternalStopAsync(ct);

        return image;
    }
}
