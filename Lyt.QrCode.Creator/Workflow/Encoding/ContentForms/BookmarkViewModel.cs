namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class BookmarkViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<BookmarkView>(qrCodeCreatorModel)
{
    public sealed record class Bookmark(string Title = "", string Url = "")
    {
        public Bookmark() : this(string.Empty, string.Empty) { }
    }

    [ObservableProperty]
    public partial string Url { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    private readonly FormValidator<Bookmark> bookmarkValidator =
        new(
            new(
                FormValidPropertyName: "FormIsValid",
                MessagePropertyName: "ValidationMessage",
                FocusFieldName: "TitleTextBox",
                FieldValidators: [Validators.TitleValidator, Validators.UrlValidator]));

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Need to clear the form when the view gets loaded so that the focus will be set 
        this.bookmarkValidator.Clear(this);
    }

    partial void OnTitleChanged(string value) => this.Submit();

    partial void OnUrlChanged(string value) => this.Submit();

    private void Submit()
    {
        try
        {
            if (!this.bookmarkValidator.Validate(this).IsValid)
            {
                return;
            }

            if (this.bookmarkValidator.HasValue)
            {
                var bookmark = this.bookmarkValidator.Value;
                var content = new QrBookmark(bookmark.Url, bookmark.Title);
                if (!this.qrCodeCreatorModel.SetContent(content))
                {
                    Debug.WriteLine("Failed to set content");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception thrown: {ex}");
        }
    }
}
