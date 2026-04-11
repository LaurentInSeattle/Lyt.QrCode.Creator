namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class MailViewModel : ViewModel<MailView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public MailViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
