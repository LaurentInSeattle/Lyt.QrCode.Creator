namespace Lyt.QrCode.Creator.Workflow.Encoding;

public sealed partial class QrCodeViewModel : ViewModel<QrCodeView> , IRecipient<ModelChangedMessage>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    [ObservableProperty]
    public partial bool HasData { get; set; } 

    public QrCodeViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.Subscribe<ModelChangedMessage>();

        // Enforce property changed 
        this.HasData = true;
        this.HasData = false ;
    }

    [RelayCommand]
    public void OnSave()
    {
        // TODO : Implement save functionality
    }

    public void Receive(ModelChangedMessage message)
        => Dispatch.OnUiThread(() => this.ReceiveOnUiThread(message));

    private void ReceiveOnUiThread(ModelChangedMessage _)
    {
        Debug.WriteLine("Model changed message received in QrCodeViewModel");
        if (this.qrCodeCreatorModel.Modules.Length == 0)
        {
            this.HasData = false;
        }

        this.HasData = true;
        this.View.ConstructGrid(
            this.qrCodeCreatorModel.Modules,
            this.qrCodeCreatorModel.Scale, 
            this.qrCodeCreatorModel.BorderSize);
    }
}
