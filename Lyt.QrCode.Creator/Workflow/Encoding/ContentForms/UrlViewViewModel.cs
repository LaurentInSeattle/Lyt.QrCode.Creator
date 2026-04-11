namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class UrlViewModel : ViewModel<UrlView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public UrlViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
    }

    [ObservableProperty]
    public partial string Url { get; set; }

    public UrlViewModel() => this.Url = string.Empty;

    [RelayCommand]
    public void Create()
    {
        var content = new QrUrl(this.Url);
        var result = Qr.EncodeToModules(content);
        if (result.Success)
        {
            var modules = result.Result;
        }
    }
}
