namespace Lyt.QrCode.Creator.Workflow.Encoding;

public sealed partial class QrCodeViewModel : 
    ViewModel<QrCodeView> , 
    IRecipient<ModelChangedMessage>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    [ObservableProperty]
    public partial bool HasData { get; set; }

    [ObservableProperty]
    public partial string EncodedString { get; set; }

    public QrCodeViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.Subscribe<ModelChangedMessage>();

        // Enforce property changed 
        this.HasData = true;
        this.HasData = false ;
        this.EncodedString = string.Empty; 
    }

    [RelayCommand]
    public void OnSave()
    {
        if (!this.HasData)
        {
            return ;
        }

        bool success = false;
        string message = string.Empty;
        try
        {
            // Save to desktop or documents depending on model settings
            string filePath = this.qrCodeCreatorModel.OutputFilePath();
            this.View.FrameGrid.SaveAsHighQualityImage(filePath);
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

    public void Receive(ModelChangedMessage message)
        => Dispatch.OnUiThread(() => this.ReceiveOnUiThread(message));

    private void ReceiveOnUiThread(ModelChangedMessage _)
    {
        // Debug.WriteLine("Model changed message received in QrCodeViewModel");
        var model = this.qrCodeCreatorModel; 
        if (model.Modules.Length == 0)
        {
            this.HasData = false;
            return;
        }

        this.HasData = true;
        this.EncodedString = model.QrCodeContent.QrString;
        var trueBrush = new SolidColorBrush(model.TrueColor);
        var falseBrush = 
            model.UseBackground ?
                new SolidColorBrush(Colors.Transparent):
                new SolidColorBrush(model.FalseColor);
        var frameForegroundBrush = new SolidColorBrush(model.FrameForegroundColor);
        var frameBackgroundBrush = new SolidColorBrush(model.FrameBackgroundColor);
        int frameSize =
            model.UseFrame ? model.FrameSize : 0 ;
        this.View.ConstructGrid(
            model.Modules,
            model.Scale, model.BorderSize, 
            frameSize,
            trueBrush, falseBrush,
            frameBackgroundBrush, frameForegroundBrush,
            model.FrameTextTop, model.FrameTextBottom, 
            model.UseLogo, model.LogoImageBytes, model.LogoSize, 
            model.UseBackground, model.BackgroundImageBytes, model.Coloring,
            model.ModuleShape);
    }
}
