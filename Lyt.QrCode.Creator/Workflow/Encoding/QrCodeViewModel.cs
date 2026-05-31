namespace Lyt.QrCode.Creator.Workflow.Encoding;

public sealed partial class QrCodeViewModel :
    ViewModel<QrCodeView>,
    IRecipient<ModelChangedMessage>,
    IRecipient<ImageGeneratedMessage>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;
    private readonly Dictionary<string, FontFamily> fontFamiliesDictionary;

    [ObservableProperty]
    public partial bool HasData { get; set; }

    [ObservableProperty]
    public partial bool ShowPrimaryImage { get; set; }

    [ObservableProperty]
    public partial bool ShowTestImage { get; set; }

    [ObservableProperty]
    public partial string EncodedString { get; set; }

    [ObservableProperty]
    public partial Bitmap? ImageSource { get; private set; }

    [ObservableProperty]
    public partial double TestImageWidth { get; private set; }

    [ObservableProperty]
    public partial double TestImageHeight { get; private set; }

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

    public QrCodeViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.Subscribe<ModelChangedMessage>();

        var fontCollection = FontManager.Current.SystemFonts;
        this.fontFamiliesDictionary = fontCollection.ToDictionary(x => x.Name, x => x);

        // Enforce property changed 
        this.HasData = true;
        this.HasData = false;
        this.ShowPrimaryImage = true;
        this.ShowPrimaryImage = false;
        this.ShowTestImage = true;
        this.ShowTestImage = false;

        this.EncodedString = string.Empty;

        this.DecodingStatusText = "No image generated yet.";
        this.DecodingStatusColor = new SolidColorBrush(Colors.Gray);
        this.ExposureColor = new SolidColorBrush(Colors.Transparent);
        this.ExposureOpacity = 0.0;
        this.TemperatureColor = new SolidColorBrush(Colors.Transparent);
        this.TemperatureOpacity = 0.0;

        this.Subscribe<ImageGeneratedMessage>();
    }

    [RelayCommand]
    public void OnSave()
    {
        if (!this.HasData)
        {
            return;
        }

        bool success = false;
        string message = string.Empty;
        try
        {
            // Save to desktop or documents depending on model settings
            string filePath = this.qrCodeCreatorModel.OutputFilePath();
            var bitmap = this.View.FrameGrid.CreateHighQualityImage();
            bitmap.Save(filePath);
            this.qrCodeCreatorModel.SetQrCodeImage(bitmap);
            message = $"QR code image saved to {filePath}";
            success = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception thrown when saving QR code image: {ex.Message}");
            message = $"Failed to save QR code image: {ex.Message}";
        }
        finally
        {
            var toaster = App.GetRequiredService<IToaster>();
            toaster.Show(
                success ? "Success" : "Error",
                message,
                success ? 10_000 : 30_000,
                success ? InformationLevel.Success : InformationLevel.Error);
        }
    }

    public void Receive(ModelChangedMessage _) => Dispatch.OnUiThread(this.ReceiveModelChangedOnUiThread);

    private void ReceiveModelChangedOnUiThread()
    {
        // Debug.WriteLine("Model changed message received in QrCodeViewModel");
        var model = this.qrCodeCreatorModel;
        if (model.Modules.Length == 0)
        {
            this.HasData = false;
            return;
        }

        this.HasData = true;
        this.ShowPrimaryImage = true;
        this.ShowTestImage = false;
        this.EncodedString = model.QrCodeContent.QrString;
        var trueBrush = new SolidColorBrush(model.TrueColor);
        var falseBrush =
            model.UseBackground ?
                new SolidColorBrush(Colors.Transparent) :
                new SolidColorBrush(model.FalseColor);
        var frameForegroundBrush = new SolidColorBrush(model.FrameForegroundColor);
        var frameBackgroundBrush = new SolidColorBrush(model.FrameBackgroundColor);
        int frameSize = model.UseFrame ? model.FrameSize : 0;

        if (! // NOT 
            (this.fontFamiliesDictionary.TryGetValue(model.FrameTextFontFamily, out FontFamily? fontFamily) && 
            fontFamily is not null)
            )
        {
            Debug.WriteLine($"Font family '{model.FrameTextFontFamily}' not found. Using default font family.");
            fontFamily = FontFamily.Default;
        }

        int topTextFontSize = model.FrameTextTopFontSize;
        int bottomTextFontSize = model.FrameTextBottomFontSize;
        int topTextFontWeight = model.FrameTextTopFontWeight;
        int bottomTextFontWeight = model.FrameTextBottomFontWeight;
        this.View.ConstructGrid(
            model.Modules,
            model.Scale, model.BorderSize,
            frameSize,
            trueBrush, falseBrush,
            frameBackgroundBrush, frameForegroundBrush,
            model.FrameTextTop, model.FrameTextBottom,
            topTextFontSize, bottomTextFontSize,
            topTextFontWeight, bottomTextFontWeight,
            fontFamily,
            model.UseLogo, model.LogoImageBytes, model.LogoSize,
            model.UseBackground, model.BackgroundImageBytes, model.Coloring, model.DarkModulesOpacity,
            model.ModuleShape);

        // We need to wait until the UI thread has finished processing the grid construction before we can create the bitmap,
        // By using DispatcherPriority.ApplicationIdle, we ensure that the bitmap is created after all other
        // UI work has been completed. 130 ms is about two frames at 60 fps, which should be enough time... 
        Schedule.OnUiThread(
            130,
            () =>
            {
                var bitmap = this.View.FrameGrid.CreateHighQualityImage();
                this.qrCodeCreatorModel.SetQrCodeImage(bitmap);
            }, 
            DispatcherPriority.ApplicationIdle);
    }

    public void Receive(ImageGeneratedMessage _) => Dispatch.OnUiThread(this.ReceiveImageGeneratedOnUiThread);

    private void ReceiveImageGeneratedOnUiThread()
    {
        var model = this.qrCodeCreatorModel;
        if (model.QrCodeImage is not Bitmap image)
        {
            return;
        }

        Debug.WriteLine("Image generated message received in Test Image ViewModel");
        this.ShowPrimaryImage = false;
        this.ShowTestImage = true;
        this.TestImageWidth = image.PixelSize.Width;
        this.TestImageHeight = image.PixelSize.Height;
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
}
