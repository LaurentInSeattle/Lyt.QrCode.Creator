namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class ImageViewModel : ViewModel<ImageView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public ImageViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
