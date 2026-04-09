namespace Lyt.QrCode.Creator.Workflow.Encoding;

public sealed partial class QrCodeViewModel : ViewModel<QrCodeView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public QrCodeViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }

}
