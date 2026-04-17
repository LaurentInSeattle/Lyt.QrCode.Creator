namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

using static Lyt.QrCode.Creator.Model.Validation.Validators;

public sealed partial class TextMessageViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<TextMessageView, TextMessageViewModel.TextMessage>(qrCodeCreatorModel, TextMessageValidator)
{
    public sealed record class TextMessage(
        string PhoneNumber = "",
        string Message = "",
        QrTextMessage.MessagingProtocol Protocol = QrTextMessage.MessagingProtocol.Sms)
    {
        public TextMessage() : this(string.Empty, string.Empty) { }
    }

    public class MessageStringValidator : AbstractValidator<string>
    {
        public MessageStringValidator()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("The text message cannot be empty.")
                .MinimumLength(4).WithMessage("The message is too short.")
                .MaximumLength(120).WithMessage("The message is too long.");
        }
    }

    private static readonly FieldValidator<string> MessageValidator =
        new(validator: new MessageStringValidator(), sourcePropertyName: "Message");

    private static readonly FormValidator<TextMessage> TextMessageValidator =
        new(focusFieldName: "PhoneNumber",
            fieldValidators:
            [
                Validators.PhoneNumber,
                MessageValidator,
                AlwaysValid<QrTextMessage.MessagingProtocol>("Protocol"),
            ]);

    [ObservableProperty]
    public partial string PhoneNumber { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Message { get; set; } = string.Empty;

    [ObservableProperty]
    public partial QrTextMessage.MessagingProtocol Protocol { get; set; } = QrTextMessage.MessagingProtocol.Sms;

    partial void OnPhoneNumberChanged(string value) => this.SubmitTextMessage();

    partial void OnMessageChanged(string value) => this.SubmitTextMessage();

    partial void OnProtocolChanged(QrTextMessage.MessagingProtocol value) => this.SubmitTextMessage();

    private void SubmitTextMessage()
        => this.Submit(value => new QrTextMessage(value.PhoneNumber, value.Message, value.Protocol));
}
