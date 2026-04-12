namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class UrlViewModel(QrCodeCreatorModel qrCodeCreatorModel) : 
    FormViewModel<UrlView>(qrCodeCreatorModel)
{
    public sealed record class WebUrl(string Url = "")
    {
        public WebUrl() : this(string.Empty) { }
    }

    [ObservableProperty]
    public partial string Url { get; set; } = string.Empty;

    private readonly FormValidator<WebUrl> webUrlValidator =
        new(
            new(
                FormValidPropertyName: "FormIsValid",
                MessagePropertyName: "ValidationMessage",
                FocusFieldName: "UrlTextBox",
                FieldValidators: [Validators.UrlValidator]));

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Need to clear the form when the view gets loaded so that the focus will be set 
        this.webUrlValidator.Clear(this);
    }

    partial void OnUrlChanged(string value) => this.Submit();

    private void Submit()
    {
        try
        {
            if (!this.webUrlValidator.Validate(this).IsValid)
            {
                return;
            }

            if (this.webUrlValidator.HasValue)
            {
                var webUrl = this.webUrlValidator.Value;
                var content = new QrUrl(webUrl.Url);
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
