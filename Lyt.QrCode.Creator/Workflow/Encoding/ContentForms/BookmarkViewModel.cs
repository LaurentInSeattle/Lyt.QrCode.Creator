namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class BookmarkViewModel(QrCodeCreatorModel qrCodeCreatorModel) : 
    FormViewModel<BookmarkView>(qrCodeCreatorModel)
{
    [ObservableProperty]
    public partial string Url { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    partial void OnTitleChanged(string value)
    {
        // Debug.WriteLine($"Title changed: {value}");
    }

    partial void OnUrlChanged(string value)
    {
        // Debug.WriteLine($"URL changed: {value}");
    }

    private void Submit()
    {
        try
        {
            var content = new QrBookmark(this.Url, this.Title);
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
