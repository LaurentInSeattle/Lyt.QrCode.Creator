namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class UrlViewModel(QrCodeCreatorModel qrCodeCreatorModel) : ViewModel<UrlView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel = qrCodeCreatorModel;

    [ObservableProperty]
    public partial string Url { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ValidationMessage { get; set; } = string.Empty;

    partial void OnUrlChanged(string value)
    {
        // Debug.WriteLine($"URL changed: {value}");
    }

    private void Submit()
    {
        try
        {
            var content = new QrUrl(this.Url);
            if (!this.qrCodeCreatorModel.SetContent(content))
            {
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception thrown: {ex}");
        }
    }
}
