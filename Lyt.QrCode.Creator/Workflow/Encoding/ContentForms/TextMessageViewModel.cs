namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class TextMessageViewModel : ViewModel<TextMessageView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public TextMessageViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
