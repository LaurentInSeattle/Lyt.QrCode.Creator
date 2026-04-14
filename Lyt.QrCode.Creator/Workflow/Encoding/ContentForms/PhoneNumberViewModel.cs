namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class PhoneNumberViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<PhoneNumberView, PhoneNumberViewModel.Phone>(qrCodeCreatorModel, PhoneNumberValidator)
{
    public sealed record class Phone(string PhoneNumber = "")
    {
        public Phone() : this(string.Empty) { }
    }

    private static readonly FormValidator<Phone> PhoneNumberValidator =
        new(
            new(
                FormValidPropertyName: "FormIsValid",
                MessagePropertyName: "ValidationMessage",
                FocusFieldName: "PhoneNumber",
                FieldValidators: [Validators.PhoneNumber]));

    [ObservableProperty]
    public partial string PhoneNumber { get; set; } = string.Empty;

    partial void OnPhoneNumberChanged(string value) => this.SubmitPhoneNumber();

    private void SubmitPhoneNumber()
        => this.Submit(value =>
        {
            var content = new QrPhoneNumber(Validators.CleanPhoneNumber(value.PhoneNumber));
            if (!this.qrCodeCreatorModel.SetContent(content))
            {
                Debug.WriteLine("Failed to set content");
            }
        });
}
