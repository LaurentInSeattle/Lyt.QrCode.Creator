namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class LogoViewModel : ViewModel<LogoView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public LogoViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
