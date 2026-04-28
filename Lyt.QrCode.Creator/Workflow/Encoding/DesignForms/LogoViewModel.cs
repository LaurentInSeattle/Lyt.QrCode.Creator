namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class LogoViewModel: ViewModel<LogoView> , IDropImageTarget
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel ;

    public LogoViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.DropViewModel = new DropViewModel(this);
        this.LogoSizeSliderValue = 100.0 * this.qrCodeCreatorModel.LogoSize;
    }

    [ObservableProperty]
    public partial DropViewModel DropViewModel { get; set; }

    [ObservableProperty]
    public partial double LogoSizeSliderValue { get; set; } 

    [ObservableProperty]
    public partial string LogoSizeString { get; set; } = string.Empty;

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.qrCodeCreatorModel.DoUseLogo ();
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.OnLogoSizeSliderValueChanged(this.LogoSizeSliderValue);
    }

    public override void Deactivate() => this.qrCodeCreatorModel.DoUseLogo (false);

    public void OnImageDrop(byte[] imageBytes) => this.qrCodeCreatorModel.SetLogo(imageBytes);

    partial void OnLogoSizeSliderValueChanged(double value)
    {
        int intValue = (int)(value +0.5);
        this.qrCodeCreatorModel.SetLogoSize(value / 100.0);
        this.LogoSizeString = string.Format("{0} %", intValue);
    }
}
