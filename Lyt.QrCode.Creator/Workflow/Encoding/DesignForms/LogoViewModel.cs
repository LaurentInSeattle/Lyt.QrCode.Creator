namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class LogoViewModel : ViewModel<LogoView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public LogoViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.qrCodeCreatorModel.UseLogo = true;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.qrCodeCreatorModel.UseLogo = false;
    }
}
