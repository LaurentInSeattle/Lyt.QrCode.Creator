namespace Lyt.QrCode.Creator.Workflow.Encoding;

public sealed partial class ContentViewModel : ViewModel<ContentView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public ContentViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }

}
