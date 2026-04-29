namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class OutputFormatViewModel : ViewModel<OutputFormatView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public OutputFormatViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
