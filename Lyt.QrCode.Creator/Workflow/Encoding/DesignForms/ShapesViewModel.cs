namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class ShapesViewModel : ViewModel<ShapesView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public ShapesViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
