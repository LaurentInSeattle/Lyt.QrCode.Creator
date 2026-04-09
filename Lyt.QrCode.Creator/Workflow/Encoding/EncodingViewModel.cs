namespace Lyt.QrCode.Creator.Workflow.Encoding;

// See https://www.qr-code-generator.com/ 


public sealed partial class EncodingViewModel : ViewModel<EncodingView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    [ObservableProperty]
    public partial QrCodeViewModel QrCodeViewModel { get; set; }

    [ObservableProperty]
    public partial ContentViewModel ContentViewModel { get; set; }

    [ObservableProperty]
    public partial FrameViewModel FrameViewModel { get; set; }

    [ObservableProperty]
    public partial LogoViewModel LogoViewModel { get; set; }

    [ObservableProperty]
    public partial ImageViewModel ImageViewModel { get; set; }

    [ObservableProperty]
    public partial ColorsViewModel ColorsViewModel { get; set; }

    [ObservableProperty]
    public partial ShapesViewModel ShapesViewModel { get; set; }

    [ObservableProperty]
    public partial SizeFormatViewModel SizeFormatViewModel { get; set; }

    public EncodingViewModel(QrCodeCreatorModel qrCodeCreatorModel) 
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.QrCodeViewModel = new (qrCodeCreatorModel);
        this.ContentViewModel = new(qrCodeCreatorModel);
        this.FrameViewModel = new(qrCodeCreatorModel);
        this.LogoViewModel = new(qrCodeCreatorModel);
        this.ImageViewModel = new(qrCodeCreatorModel);
        this.ColorsViewModel = new(qrCodeCreatorModel);
        this.ShapesViewModel = new(qrCodeCreatorModel);
        this.SizeFormatViewModel = new(qrCodeCreatorModel);
    }
}