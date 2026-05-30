namespace Lyt.QrCode.Creator.Workflow.Encoding;

public sealed partial class TestImageViewModel :
    ViewModel<TestImageView>,
    IRecipient<ImageGeneratedMessage>

{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    [ObservableProperty]
    public partial Bitmap? ImageSource { get; private set; }

    [ObservableProperty]
    public partial string DecodingStatusText { get; private set; }

    [ObservableProperty]
    public partial SolidColorBrush DecodingStatusColor { get; private set; }

    [ObservableProperty]
    public partial string RawContent { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ContentType { get; set; } = string.Empty;

    [ObservableProperty]
    public partial SolidColorBrush ExposureColor { get; private set; }

    [ObservableProperty]
    public partial double ExposureOpacity { get; private set; }

    [ObservableProperty]
    public partial SolidColorBrush TemperatureColor { get; private set; }

    [ObservableProperty]
    public partial double TemperatureOpacity { get; private set; }

    public TestImageViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.DecodingStatusText = "No image generated yet.";
        this.DecodingStatusColor = new SolidColorBrush(Colors.Gray);
        this.ExposureColor = new SolidColorBrush(Colors.Transparent);
        this.ExposureOpacity = 0.0;
        this.TemperatureColor = new SolidColorBrush(Colors.Transparent);
        this.TemperatureOpacity = 0.0;

        this.Subscribe<ImageGeneratedMessage>();
    }

    public void Receive(ImageGeneratedMessage _) => Dispatch.OnUiThread(() => this.ReceiveOnUiThread());

    private void ReceiveOnUiThread()
    {
        var model = this.qrCodeCreatorModel;
        if ( model.QrCodeImage is not Bitmap image)
        {
            return;
        }

        Debug.WriteLine("Image generated message received in Test Image ViewModel");
        this.ImageSource = image;
        _ = this.TryDecode(image);
    }

    private async Task TryDecode(Bitmap bitmap)
    {
        var sourceImage = ImagingUtilities.BitmapToSourceImage(bitmap);
        DecodeResult result = Qr.Decode(sourceImage);
        if (result.Success)
        {
            Debug.WriteLine($"QR code decoded");

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
            this.DecodingStatusText = "Image decoded successfully.";
            this.DecodingStatusColor = new SolidColorBrush(Colors.Green);
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
            this.DecodingStatusText = "Failed to decode the image.";
            this.DecodingStatusColor = new SolidColorBrush(Colors.Firebrick);
        }
    }

    //private async Task DecodeImageAsync(Bitmap image)
    //{
    //    try
    //    {
    //        var reader = new BarcodeReader();
    //        var result = await Task.Run(() => reader.Decode(image));
    //        if (result != null)
    //        {
    //            this.DecodingStatusText = $"Decoded content: {result.Text}";
    //            this.DecodingStatusColor = new SolidColorBrush(Colors.Green);
    //        }
    //        else
    //        {
    //            this.DecodingStatusText = "Failed to decode the image.";
    //            this.DecodingStatusColor = new SolidColorBrush(Colors.Red);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine($"Exception during decoding: {ex.Message}");
    //        this.DecodingStatusText = $"Error during decoding: {ex.Message}";
    //        this.DecodingStatusColor = new SolidColorBrush(Colors.Red);
    //    }
    //}

    //private void UpdateDecodingStatus()
    //{
    //    this.DecodingStatusText = text;
    //    this.DecodingStatusColor = new SolidColorBrush(color);
    //    this.DecodingStatusText = "Image generated successfully.";
    //    this.DecodingStatusColor = new SolidColorBrush(Colors.Green);
    //}
}