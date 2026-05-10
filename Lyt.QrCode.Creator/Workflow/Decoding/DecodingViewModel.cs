namespace Lyt.QrCode.Creator.Workflow.Decoding;

using SkiaSharp;

using static Lyt.QrCode.Creator.Utilities.SkiaExtensions;

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

        try
        {
            await this.selectedDevice!.StartAsync();
            await Task.Delay(240); // Let it run for a while to capture some frames.
            if (this.selectedDevice.IsRunning)
            {
                Debug.WriteLine($"Capture started.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to start capture: {ex}");
            this.selectedDevice = null;
        }
    }

    private async Task StopCapture()
    {
        if (!this.CanCapture)
        {
            Debug.WriteLine($"Cannot start capture, no device or characteristics found or selected.");
            return;
        }

        try
        {
            await this.selectedDevice!.StopAsync();
            await Task.Delay(120); // Let it run for a while to capture some frames.
            if (this.selectedDevice.IsRunning)
            {
                Debug.WriteLine($"Cannot stop capture.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to start capture: {ex}");
            this.selectedDevice = null;
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
        try
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
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to detect capture devices: {ex}");
            this.selectedDevice = null;
        }
    }

    private async Task OnPixelBufferArrivedAsync(PixelBufferScope bufferScope)
    {
        try
        {
            // This thread context is NOT the UI thread:  refer image data binary directly.
            ArraySegment<byte> image = bufferScope.Buffer.ReferImage();

            // Decode image data to a skia bitmap:
            var bitmap = SKBitmap.Decode(image);

            // Switch to UI thread
            if (bitmap is null)
            {
                Debug.WriteLine($"Failed to decode bitmap from image data.");
                return;
            }

            Dispatch.OnUiThread(() => this.UpdateImageAndStatistics(bitmap.ToAvaloniaIImage()));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to detect capture devices: {ex}");
            this.selectedDevice = null;
        }
        finally
        {
            // Ensure the pixel buffer is released in case of any exception.
            // `bitmap` is copied, so we can release pixel buffer now.
            // => release even if bitmap decoding failed, since we won't use the pixel buffer anymore.
            bufferScope.ReleaseNow();
        }
    }

    private void UpdateImageAndStatistics(IImage? image)
    {
        // Dispose previous frame if exists.
        if (this.Image is AvaloniaImage oldImage)
        {
            oldImage.Dispose();
        }

        // Update the image.
        this.Image = image;

        // TODO: Update statistics.
        //var countFrames = Interlocked.Increment(ref this.countFrames);
        //var realFps = countFrames / timestamp.TotalSeconds;
        //var fpsByIndex = frameIndex / timestamp.TotalSeconds;
        //this.Statistics2 = $"FPS={realFps:F3}/{fpsByIndex:F3}";
        //this.Statistics3 = $"SKBitmap={bitmap.Width}x{bitmap.Height} [{bitmap.ColorType}]";
    }
}