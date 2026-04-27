namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class LogoViewModel: ViewModel<LogoView> , IDropImageTarget
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel ;

    public LogoViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.DropViewModel = new DropViewModel(this);
    }

    [ObservableProperty]
    public partial DropViewModel DropViewModel { get; set; }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.qrCodeCreatorModel.DoUseLogo ();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.qrCodeCreatorModel.DoUseLogo (false);
    }

    public void OnImageDrop(byte[] imageBytes)
    {

    }
}
