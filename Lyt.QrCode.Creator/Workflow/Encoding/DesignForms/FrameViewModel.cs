namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class FrameViewModel : ViewModel<FrameView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public FrameViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
