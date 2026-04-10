namespace Lyt.QrCode.Creator.Workflow.Encoding;

public sealed partial class TestImageViewModel : ViewModel<TestImageView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public TestImageViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }

}
