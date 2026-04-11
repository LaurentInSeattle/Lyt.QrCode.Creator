namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class PhoneNumberViewModel : ViewModel<PhoneNumberView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public PhoneNumberViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }
}
