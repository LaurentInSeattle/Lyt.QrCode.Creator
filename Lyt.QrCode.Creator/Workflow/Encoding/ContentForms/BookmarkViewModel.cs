namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class BookmarkViewModel : ViewModel<BookmarkView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    [ObservableProperty]
    public partial string Url { get; set; } 

    [ObservableProperty]
    public partial string Title { get; set; }

    public BookmarkViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.Url = string.Empty;
        this.Title = string.Empty;
    }

    [RelayCommand]
    public void Create()
    {
        var content = new QrBookmark(this.Url, this.Title);
        if (!this.qrCodeCreatorModel.SetContent(content))
        {
        } 
    }
}
