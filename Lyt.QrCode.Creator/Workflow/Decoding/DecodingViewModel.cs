namespace Lyt.QrCode.Creator.Workflow.Decoding;

using SkiaSharp;
using Lyt.QrCode.Image;

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
    public partial bool IsDecoded { get; set; }

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

    [ObservableProperty]
    public partial string RawContent { get; set; }

    [ObservableProperty]
    public partial string ContentType { get; set; }

    public DecodingViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.CaptureStatus = string.Empty;
        this.CaptureDeviceInfo = string.Empty;
        this.RawContent = string.Empty;
        this.ContentType = string.Empty;
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        this.Image = null; // Clear the image.
        this.IsCapturing = false;
        this.IsDecoded = false;

        // Start detection of capture devices: Fire and forget,
        // we will update the view when devices are detected and selected.
        this.RawContent = string.Empty;
        this.ContentType = string.Empty;
        this.CaptureStatus = "Initializing...";
        this.CaptureDeviceInfo = string.Empty;
        _ = this.DetectCaptureDevices();
    }

    [RelayCommand]
    public void OnRestart()
    {
        this.IsDecoded = false;
        this.RawContent = string.Empty;
        this.ContentType = string.Empty;
        Dispatch.OnUiThread(() => this.HideAllMarkers());

        // Start capture: Fire and forget,
        _ = this.StartCapture();
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.OnRestart();
    }

    public override void Deactivate()
    {
        base.Deactivate();

        // Stop capture: Fire and forget,
        _ = this.StopCapture(andClearImage: true);
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
            this.CaptureStatus = "Failed to start video capture: no device.";
            return;
        }

        try
        {
            await Task.Delay(240);
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
            this.CaptureStatus = $"Failed to start capture. (Exception: {ex.Message})"; 
            this.selectedDevice = null;
        }
    }

    private async Task StopCapture(bool andClearImage = false)
    {
        if (andClearImage)
        {
            // Clear the image when stopping capture, if requested
            this.Image = null;
        }

        if (!this.CanCapture)
        {
            Debug.WriteLine($"Cannot stop capture, no device or characteristics found or selected.");
            return;
        }

        string captureStatus = string.Empty;
        try
        {
            await this.selectedDevice!.StopAsync();
            await Task.Delay(120); // Let it run for a while to capture some frames.
            if (this.selectedDevice.IsRunning)
            {
                Debug.WriteLine("Cannot stop capture.");
                captureStatus = "Failed to stop capture. Still running?";
            }
            else
            {
                Debug.WriteLine("Capture stopped.");
                captureStatus = "Capture stopped.";
                this.IsCapturing = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to stop capture: {ex}");
            captureStatus = $"Failed to stop capture. (Exception: {ex.Message})";
            this.selectedDevice = null;
        }
        finally
        {
            Dispatch.OnUiThread(() => { this.CaptureStatus = captureStatus; });
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

            // Only MediaFoundation devices.
            descriptors = [.. devices.Enumerate().Where(d => d.DeviceType == DeviceTypes.MediaFoundation)];
            // Only DirectShow devices.
            // descriptors = [.. devices.EnumerateDescriptors().Where(d => d.DeviceType == DeviceTypes.DirectShow)];

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
                characteristics
                    .OrderByDescending(c => c.Width * c.Height)
                    .ThenByDescending(c => (double)c.FramesPerSecond)
                    .ToList();
            this.selectedCharacteristics = sorted[0];
            Debug.WriteLine($"Selected capture device: {this.selectedDeviceDescriptor}, {this.selectedCharacteristics}");
            captureStatus = "Not capturing";
            this.selectedDevice =
                await this.selectedDeviceDescriptor.OpenAsync(this.selectedCharacteristics, this.OnNewFrame);
            captureDeviceInfo =
                $"Selected capture device: {this.selectedDeviceDescriptor.Description}, {this.selectedCharacteristics}";
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

            Dispatch.OnUiThread(() => this.UpdateImageAndStatistics(bitmap.ToWriteableBitmap()));
            ++this.frameCounter;

            // Try to decode QR code if not decoded yet.
            // We have a UI button to reset that flag.
            if (!this.IsDecoded)
            {
                // Launch decode after 60 frames...
                // that's should be around 2 seconds if the camera runs at 30 FPS, or around 1 second if it runs at 60 FPS...
                if (this.frameCounter > 60)
                {
                    // ... and at most once per two seconds, to avoid too much decoding work.
                    var now = DateTime.Now;
                    if (now - this.lastFrameTimestamp > TimeSpan.FromSeconds(2))
                    {
                        // Fire and Forget Decode the source image to get the QR code content
                        // This will later update the view with the results, if any.
                        _ = this.TryDecode(ToSourceImage(bitmap));
                        this.lastFrameTimestamp = now;
                    }
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
            bufferScope.ReleaseNow();
        }
    }

    private void UpdateImageAndStatistics(IImage? image)
    {
        // Dispose previous frame if exists.
        if (this.Image is WriteableBitmap oldImage)
        {
            oldImage.Dispose();
        }

        // Update the image.
        this.Image = image;
    }

    private static SourceImage ToSourceImage(SKBitmap bitmap)
    {
        byte[] srcPixels = bitmap.Bytes;
        byte[] dstPixels = new byte[srcPixels.Length];
        Buffer.BlockCopy(srcPixels, 0, dstPixels, 0, srcPixels.Length);
        PixelFormat pixelFormat = 
            bitmap.ColorType == SKColorType.Bgra8888 ? PixelFormat.BGRA32 : PixelFormat.RGBA32;
        return 
            new SourceImage(bitmap.Width, bitmap.Height, bitmap.RowBytes, pixelFormat, dstPixels);
    }

    private void OnDetect(QrPixelPoint qrPixelPoint)
    {
        if (qrPixelPoint.IsValid)
        {
            Debug.WriteLine($"QR code point detected");
            Dispatch.OnUiThread(() => this.ShowQrPixelPoint(qrPixelPoint));
        }
    }

    // Show the detected QR code point on the view.
    private void ShowQrPixelPoint(QrPixelPoint qrPixelPoint)
        => this.View.AddMarker(qrPixelPoint.X, qrPixelPoint.Y);

    // Hide all previously detected QR code points on the view.
    private void HideAllMarkers() => this.View.ClearMarkers();

    private async Task TryDecode(SourceImage sourceImage)
    {
        Dispatch.OnUiThread(() => this.HideAllMarkers());
        DecodeParameters parameters = new();
        DecodeResult result = Qr.Decode(sourceImage, this.OnDetect, parameters);
        if (result.Success)
        {
            Debug.WriteLine($"QR code decoded");

            // Freeze the image to show the detected QR code, and stop capture to save resources.
            // This will also show the button to scan another code 
            _ = this.StopCapture();
            this.IsDecoded = true;

            if (result.IsParsed)
            {
                Debug.WriteLine($"QR code content: {result.ParsedObject.GetType().Name}");
            }
        }
        else
        {
            Debug.WriteLine($"QR code decoding failed");
        }

        // Update the UI 
        Dispatch.OnUiThread(() => this.UpdateUiOnDecoding(result));
    }

    private void UpdateUiOnDecoding(DecodeResult result)
    {
        bool success = result.Success;
        if (success)
        {
            if (result.IsDetected)
            {
                this.CaptureStatus = "Not capturing.";
                this.CalculateAndShowDetectionSquare(result);
            } 

            this.RawContent = result.Text;
            if (result.IsParsed)
            {
                string contentType = result.ParsedObject.GetType().Name;
                Debug.WriteLine($"QR code content: {contentType}");
                this.ContentType = contentType;
            }
            else if (QrUrl.TryParse(result.Text, out QrUrl? qrUrl))
            {
                Debug.WriteLine($"QR code content is a URL: {qrUrl}");
                this.ContentType = "Web Page (URL)";
            }
            else
            {
                Debug.WriteLine($"QR code content is plain text.");
                this.ContentType = "Plain Text";
            }
        }
        else
        {
            this.RawContent = string.Empty;
            this.ContentType = string.Empty;
        }
    }

    private void CalculateAndShowDetectionSquare(DecodeResult decodeResult) 
    {
        QrPixelPoint topRight = decodeResult.TopRight;
        QrPixelPoint topLeft = decodeResult.TopLeft;
        QrPixelPoint bottomLeft = decodeResult.BottomLeft;
        double centerX = (topRight.X + bottomLeft.X) / 2.0;
        double centerY = (topRight.Y + bottomLeft.Y) / 2.0;
        double sqrtOfTwo = Math.Sqrt(2.0);
        double width = 2.0 * Math.Abs(topRight.X - bottomLeft.X)  / sqrtOfTwo;
        double deltaX = topLeft.X - topRight.X;
        double deltaY = topLeft.Y - topRight.Y;
        double angle = Math.Atan2(deltaY, deltaX);
        
        // add some room (15%) for the quiet zone
        double size = width * 1.15; 
        this.View.AddDetectionSquare(centerX, centerY, size, angle);
    }
}