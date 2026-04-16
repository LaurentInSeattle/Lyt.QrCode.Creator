namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class BookmarkViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<BookmarkView, BookmarkViewModel.Bookmark>(qrCodeCreatorModel, BookmarkValidator)
{
    public sealed record class Bookmark(string Title = "", string Url = "")
    {
        public Bookmark() : this(string.Empty, string.Empty) { }
    }

    private static readonly FormValidator<Bookmark> BookmarkValidator =
        new( focusFieldName: "TitleTextBox",
             fieldValidators: [Validators.TitleValidator, Validators.UrlValidator]);
    
    [ObservableProperty]
    public partial string Url { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    partial void OnTitleChanged(string value) => this.SubmitBookmark();

    partial void OnUrlChanged(string value) => this.SubmitBookmark();

    private void SubmitBookmark()
        => this.Submit(value =>
         {
             var content = new QrBookmark(value.Url, value.Title);
             if (!this.qrCodeCreatorModel.SetContent(content))
             {
                 Debug.WriteLine("Failed to set content");
             }
         });
}
