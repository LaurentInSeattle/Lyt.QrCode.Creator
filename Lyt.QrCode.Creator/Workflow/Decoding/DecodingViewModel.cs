namespace Lyt.QrCode.Creator.Workflow.Decoding;

using SkiaSharp;

public sealed partial class DecodingViewModel(QrCodeCreatorModel qrCodeCreatorModel) : ViewModel<DecodingView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel = qrCodeCreatorModel;

    private CaptureDevice? selectedDevice;

    private CaptureDeviceDescriptor? selectedDeviceDescriptor;

    private VideoCharacteristics? selectedCharacteristics;

    //[ObservableProperty]
    //public partial QrCodeViewModel QrCodeViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial IImage? Image { get; set; }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Start detection of capture devices: Fire and forget,
        // we will update the view when devices are detected and selected.
        _ = this.DetectCaptureDevices();
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);

        // Start capture: Fire and forget,
        _ = this.StartCapture();
    }

    public override void Deactivate()
    {
        base.Deactivate();

        // Stop capture: Fire and forget,
        _ = this.StopCapture();
    }

    private bool CanCapture
        => this.selectedDevice is not null &&
            this.selectedDeviceDescriptor is not null &&
            this.selectedCharacteristics is not null;

    private async Task StartCapture()
    {
        if (!this.CanCapture)
        {
            Debug.WriteLine($"Cannot start capture, no device or characteristics found or selected.");
            return;
        }

        await this.selectedDevice!.StartAsync();
        await Task.Delay(240); // Let it run for a while to capture some frames.
        if (this.selectedDevice.IsRunning)
        {
            Debug.WriteLine($"Capture started.");
        }
    }

    private async Task StopCapture()
    {
        if (!this.CanCapture)
        {
            Debug.WriteLine($"Cannot start capture, no device or characteristics found or selected.");
            return;
        }

        await this.selectedDevice!.StopAsync();
        await Task.Delay(120); // Let it run for a while to capture some frames.
        if (this.selectedDevice.IsRunning)
        {
            Debug.WriteLine($"Cannot stop capture.");
        }
    }

    private async Task DetectCaptureDevices()
    {
        Platforms platform = Platform.Current;
        if (platform == Platforms.Windows)
        {
            await this.DetectCaptureDevicesWindows();
        }
        else
        {
            // TODO: Implement for other platforms
        }
    }

    private async Task DetectCaptureDevicesWindows()
    {
        CaptureDevices devices = new();
        List<CaptureDeviceDescriptor> descriptors = [];

        // Only DirectShow devices.
        descriptors = [.. devices.EnumerateDescriptors().Where(d => d.DeviceType == DeviceTypes.DirectShow)];

        // pickup first device FOR NOW ,
        // TODO: Allow user to select device
        var firstDevice = descriptors.FirstOrDefault();
        if (firstDevice == null)
        {
            Debug.WriteLine($"Could not detect any capture interfaces.");
            return;
        }

        this.selectedDeviceDescriptor = firstDevice;

        // get characteristics 
        var characteristics = this.selectedDeviceDescriptor.Characteristics;
        if (characteristics.Length == 0)
        {
            Debug.WriteLine($"Could not select color format characteristics.");
            return;
        }

        // Select best characteristics, first by size, then by frame rate
        var sorted =
            (from c in characteristics
             orderby c.Width * c.Height descending
             orderby (double)c.FramesPerSecond descending
             select c).ToList();
        this.selectedCharacteristics = sorted[0];
        Debug.WriteLine($"Selected capture device: {this.selectedDeviceDescriptor}, {this.selectedCharacteristics}");

        this.selectedDevice =
            await this.selectedDeviceDescriptor.OpenAsync(
                this.selectedCharacteristics,
                this.OnPixelBufferArrivedAsync);
    }

    private async Task OnPixelBufferArrivedAsync(PixelBufferScope bufferScope)
    {
        // this thread context is NOT UI thread.
        // refer image data binary directly.
        ArraySegment<byte> image = bufferScope.Buffer.ReferImage();

        // Decode image data to a bitmap:
        var bitmap = SKBitmap.Decode(image);

        // Capture statistics variables.
        //var countFrames = Interlocked.Increment(ref this.countFrames);
        //var frameIndex = bufferScope.Buffer.FrameIndex;
        //var timestamp = bufferScope.Buffer.Timestamp;

        // `bitmap` is copied, so we can release pixel buffer now.
        bufferScope.ReleaseNow();

        // Switch to UI thread
        if (bitmap == null)
        {
            Debug.WriteLine($"Failed to decode bitmap from image data.");
            return;
        }

        Dispatch.OnUiThread(() => this.UpdateImageAndStatistics(bitmap.ToAvaloniaImage()));
    }

    private void UpdateImageAndStatistics(IImage? image)
    {
        // Update the image.
        this.Image = image;

        // Update statistics.
        //var realFps = countFrames / timestamp.TotalSeconds;
        //var fpsByIndex = frameIndex / timestamp.TotalSeconds;
        //this.Statistics1 = $"Frame={countFrames}/{frameIndex}";
        //this.Statistics2 = $"FPS={realFps:F3}/{fpsByIndex:F3}";
        //this.Statistics3 = $"SKBitmap={bitmap.Width}x{bitmap.Height} [{bitmap.ColorType}]";
    }
}