namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class FrameViewModel : ViewModel<FrameView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public FrameViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.qrCodeCreatorModel.UseFrame = true;
    } 

    public override void Deactivate()
    {
        base.Deactivate();
        this.qrCodeCreatorModel.UseFrame = false;
    } 

}
