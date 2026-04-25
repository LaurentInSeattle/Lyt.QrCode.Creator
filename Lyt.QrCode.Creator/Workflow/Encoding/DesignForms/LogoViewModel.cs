namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class LogoViewModel(QrCodeCreatorModel qrCodeCreatorModel) : ViewModel<LogoView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel = qrCodeCreatorModel;

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
}
