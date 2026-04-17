namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class UrlViewModel(QrCodeCreatorModel qrCodeCreatorModel) : 
    FormViewModel<UrlView, UrlViewModel.WebUrl>(qrCodeCreatorModel, WebUrlValidator)
{
    public sealed record class WebUrl(string Url = "")
    {
        public WebUrl() : this(string.Empty) { }
    }

    [ObservableProperty]
    public partial string Url { get; set; } = string.Empty;

    private static readonly FormValidator<WebUrl> WebUrlValidator =
        new(focusFieldName: "UrlTextBox", fieldValidators: [Validators.UrlValidator]);

    partial void OnUrlChanged(string value) => base.Submit( value => new QrUrl(value.Url));
}
