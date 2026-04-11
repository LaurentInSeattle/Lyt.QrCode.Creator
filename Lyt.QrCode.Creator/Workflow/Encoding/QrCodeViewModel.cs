namespace Lyt.QrCode.Creator.Workflow.Encoding;

public sealed partial class QrCodeViewModel : ViewModel<QrCodeView> , IRecipient<ModelChangedMessage>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public QrCodeViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.Subscribe<ModelChangedMessage>();
    }

    public void Receive(ModelChangedMessage message)
    {
        Debug.WriteLine("Model changed message received in QrCodeViewModel");
    }
}
