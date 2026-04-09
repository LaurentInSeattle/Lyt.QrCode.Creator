namespace Lyt.QrCode.Creator.Workflow.Encoding;

// See https://www.qr-code-generator.com/ 


public sealed partial class EncodingViewModel : ViewModel<EncodingView>
{
    [ObservableProperty]
    public partial QrCodeViewModel QrCodeViewModel { get; set; }

    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public EncodingViewModel(QrCodeCreatorModel qrCodeCreatorModel) 
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.QrCodeViewModel = new QrCodeViewModel(qrCodeCreatorModel);
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
    }
}
