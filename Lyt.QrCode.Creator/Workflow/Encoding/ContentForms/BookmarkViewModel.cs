namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class BookmarkViewModel : ViewModel<BookmarkView>
{
    [ObservableProperty]
    public partial string Url { get; set; } 

    [ObservableProperty]
    public partial string Title { get; set; }

    public BookmarkViewModel()
    {
        this.Url = string.Empty;
        this.Title = string.Empty;
    }

    [RelayCommand]
    public void Create()
    {
        var content = new QrBookmark(this.Url, this.Title);
        var result = Qr.EncodeToModules(content);
        if (result.Success)
        {
            var modules = result.Result; 
        } 
    }
}
