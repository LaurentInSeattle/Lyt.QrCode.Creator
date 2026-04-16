namespace Lyt.QrCode.Creator.Workflow.Encoding;

// See https://www.qr-code-generator.com/ 


public sealed partial class EncodingViewModel(QrCodeCreatorModel qrCodeCreatorModel) : ViewModel<EncodingView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel = qrCodeCreatorModel;

    [ObservableProperty]
    public partial QrCodeViewModel QrCodeViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial TestImageViewModel TestImageViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial ContentViewModel ContentViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial FrameViewModel FrameViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial LogoViewModel LogoViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial ImageViewModel ImageViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial ColorsViewModel ColorsViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial ShapesViewModel ShapesViewModel { get; set; } = new(qrCodeCreatorModel);

    [ObservableProperty]
    public partial SizeFormatViewModel SizeFormatViewModel { get; set; } = new(qrCodeCreatorModel);

    public override void OnViewLoaded() 
    {
        base.OnViewLoaded();
        this.View.ContentContainer.ToggleCollapse();
        this.View.QrCodeContainer.ToggleCollapse();
    }
}