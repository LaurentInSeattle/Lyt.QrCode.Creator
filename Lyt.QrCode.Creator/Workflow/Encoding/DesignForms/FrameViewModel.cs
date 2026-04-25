namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class FrameViewModel(QrCodeCreatorModel qrCodeCreatorModel) : ViewModel<FrameView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel = qrCodeCreatorModel;

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.qrCodeCreatorModel.DoUseFrame(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.qrCodeCreatorModel.DoUseFrame(false);
    }
}
