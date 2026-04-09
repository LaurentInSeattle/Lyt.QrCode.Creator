namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class ColorsViewModel : ViewModel<ColorsView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public ColorsViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
