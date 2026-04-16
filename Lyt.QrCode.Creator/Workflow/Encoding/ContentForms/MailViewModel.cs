namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class MailViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<MailView, MailViewModel.Mail>(qrCodeCreatorModel, MailValidator)
{
    public sealed record class Mail(string EmailAddress = "")
    {
        public Mail() : this(string.Empty) { }
    }

    private static readonly FormValidator<Mail> MailValidator =
        new(focusFieldName: "EmailAddress", fieldValidators: [Validators.EmailAddress]);

    [ObservableProperty]
    public partial string EmailAddress { get; set; } = string.Empty;

    partial void OnEmailAddressChanged(string value) => this.SubmitEmailAddress();

    private void SubmitEmailAddress()
        => this.Submit(value =>
        {
            var content = new QrMail(value.EmailAddress);
            if (!this.qrCodeCreatorModel.SetContent(content))
            {
                Debug.WriteLine("Failed to set content");
            }
        });
}
