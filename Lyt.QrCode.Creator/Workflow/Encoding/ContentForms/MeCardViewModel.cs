namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class MeCardViewModel : ViewModel<MeCardView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public MeCardViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
