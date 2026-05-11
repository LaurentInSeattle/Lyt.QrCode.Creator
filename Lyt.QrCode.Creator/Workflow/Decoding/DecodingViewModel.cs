namespace Lyt.QrCode.Creator.Workflow.Decoding;

using Lyt.QrCode.Image;

using SkiaSharp;

using static Lyt.QrCode.Creator.Utilities.SkiaExtensions;

public sealed partial class DecodingViewModel : ViewModel<DecodingView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    private CaptureDevice? selectedDevice;

    private CaptureDeviceDescriptor? selectedDeviceDescriptor;

    private VideoCharacteristics? selectedCharacteristics;

    private DateTime lastFrameTimestamp;
    private int frameCounter;

    [ObservableProperty]
    public partial bool IsCapturing { get; set; }

    [ObservableProperty]
    public partial string CaptureStatus { get; set; }

    [ObservableProperty]
    public partial string CaptureDeviceInfo { get; set; }

    [ObservableProperty]
    public partial IImage? Image { get; set; }

    [ObservableProperty]
    public partial double ImageWidth { get; set; }

    [ObservableProperty]
    public partial double ImageHeight { get; set; }

    public DecodingViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.CaptureStatus = string.Empty;
        this.CaptureDeviceInfo = string.Empty;
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        this.Image = null; // Clear the image.
        this.IsCapturing = false;

        // Start detection of capture devices: Fire and forget,
        // we will update the view when devices are detected and selected.
        this.CaptureStatus = "Initializing...";
        this.CaptureDeviceInfo = string.Empty;
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
        this.Image = null; // Clear the image.
        this.IsCapturing = false;

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
                this.lastFrameTimestamp = DateTime.Now;
                this.frameCounter = 0;
                this.IsCapturing = true;
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
        this.Image = null; // Clear the image when stopping capture.

        if (!this.CanCapture)
        {
            Debug.WriteLine($"Cannot stop capture, no device or characteristics found or selected.");
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
            else
            {
                Debug.WriteLine($"Capture stopped.");
                this.IsCapturing = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to stop capture: {ex}");
            this.selectedDevice = null;
        }
    }

    private async Task DetectCaptureDevices()
    {
        if (OperatingSystem.IsWindows())
        {
            // Windows specific 
            await this.DetectCaptureDevicesWindows();
        }
        else if (OperatingSystem.IsMacOS()) { /* macOS specific logic */ }
        else if (OperatingSystem.IsLinux()) { /* Linux specific logic */ }
        else if (OperatingSystem.IsAndroid()) { /* Android specific logic */ }
        else if (OperatingSystem.IsIOS()) { /* iOS specific logic */ }
        else if (OperatingSystem.IsBrowser()) { /* WebAssembly logic */ }
    }

    private async Task DetectCaptureDevicesWindows()
    {
        string captureStatus = string.Empty;
        string captureDeviceInfo = string.Empty;
        int imageWidth = 0;
        int imageHeight = 0;
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
                captureStatus = "Could not detect any video capture device.";
                return;
            }

            this.selectedDeviceDescriptor = firstDevice;

            // get characteristics 
            var characteristics = this.selectedDeviceDescriptor.Characteristics;
            if (characteristics.Length == 0)
            {
                Debug.WriteLine($"Could not select color format characteristics.");
                captureStatus = "Could not find any video color format characteristics.";
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
            this.CaptureStatus = "Not capturing";
            this.selectedDevice =
                await this.selectedDeviceDescriptor.OpenAsync(this.selectedCharacteristics, this.OnNewFrame);
            captureDeviceInfo = $"Selected capture device: {this.selectedDeviceDescriptor}, {this.selectedCharacteristics}";
            imageWidth = this.selectedCharacteristics.Width;
            imageHeight = this.selectedCharacteristics.Height;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to detect capture devices: {ex}");
            captureStatus = "Failed to detect any video capture device.";
            this.selectedDevice = null;
        }
        finally
        {
            // No matter what, we should update the view on the UI thread,
            // with the latest image size, status and device info.
            Dispatch.OnUiThread(() =>
            {
                this.CaptureStatus = captureStatus;
                this.CaptureDeviceInfo = captureDeviceInfo;
                this.ImageWidth = imageWidth;
                this.ImageHeight = imageHeight;
            });
        }
    }

    private async Task OnNewFrame(PixelBufferScope bufferScope)
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

            Dispatch.OnUiThread(() => this.UpdateImageAndStatistics(bitmap.ToAvaloniaIImage(), bufferScope));
            Interlocked.Increment(ref this.frameCounter);

            // Launch decode after 60 frames...
            // (that's should be around 2 seconds if the camera runs at 30 FPS, or around 1 second if it runs at 60 FPS)...
            if (this.frameCounter > 60)
            {
                // ... and at most once per two seconds, to avoid too much decoding work.
                var now = DateTime.Now;
                if (now - this.lastFrameTimestamp > TimeSpan.FromSeconds(2))
                {
                    var sourceImage = this.ToSourceImage(bitmap);

                    // Fire and Forget Decode the source image to get the QR code content
                    // This will later update the view with the results, if any.
                    _ = this.TryDecode(sourceImage);
                    this.lastFrameTimestamp = now;
                }
            }
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
            // bufferScope.ReleaseNow();
        }
    }

    private void UpdateImageAndStatistics(IImage? image, PixelBufferScope bufferScope)
    {
        // Dispose previous frame if exists.
        if (this.Image is AvaloniaImage oldImage)
        {
            oldImage.Dispose();
        }

        // Update the image.
        this.Image = image;

        // Ensure the pixel buffer is released in case of any exception.
        // `bitmap` is copied, so we can release pixel buffer now.
        // => release even if bitmap decoding failed, since we won't use the pixel buffer anymore.
        bufferScope.ReleaseNow();

        // Update statistics.
        //var realFps = countFrames / timestamp.TotalSeconds;
        //var fpsByIndex = frameIndex / timestamp.TotalSeconds;
        //this.Statistics2 = $"FPS={realFps:F3}/{fpsByIndex:F3}";
        //this.Statistics3 = $"SKBitmap={bitmap.Width}x{bitmap.Height} [{bitmap.ColorType}]";
    }

    private SourceImage ToSourceImage(SKBitmap bitmap)
    {
        byte[] srcPixels = bitmap.Bytes;
        byte[] destPixels = new byte[srcPixels.Length];
        Buffer.BlockCopy(srcPixels, 0, destPixels, 0, srcPixels.Length);
        return new SourceImage(
            bitmap.Width, bitmap.Height, bitmap.RowBytes, PixelFormat.BGRA32, destPixels);
    }

    private void OnDetect(QrPixelPoint qrPixelPoint)
    {
        if (qrPixelPoint.IsValid)
        {
            Debug.WriteLine($"QR code point detected");
            Dispatch.OnUiThread(() => this.ShowQrPixelPoint(qrPixelPoint));
        }
    }

    private void ShowQrPixelPoint(QrPixelPoint qrPixelPoint)
    {
        // TODO : Show the detected QR code point on the view.
    }

    private void HideAllQrPixelPoint()
    {
        // TODO : Hide all previously detected QR code points on the view.
    }

    private async Task TryDecode(SourceImage sourceImage)
    {
        Dispatch.OnUiThread(() => this.HideAllQrPixelPoint());
        DecodeParameters parameters = new ();
        DecodeResult result = Qr.Decode(sourceImage, this.OnDetect, parameters);
        if ( result.Success)
        {
            // TODO 
            Debug.WriteLine($"QR code decoded");
        }
        else
        {
            Debug.WriteLine($"QR code decoding failed");
        }
    }
}