namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class SizeFormatViewModel : ViewModel<SizeFormatView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public SizeFormatViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
