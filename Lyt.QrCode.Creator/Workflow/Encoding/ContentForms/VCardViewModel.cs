namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class VCardViewModel : ViewModel<VCardView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public VCardViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
