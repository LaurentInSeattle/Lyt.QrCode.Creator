namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class WifiViewModel : ViewModel<WifiView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public WifiViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }

}
