namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class PhoneNumberViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<PhoneNumberView, PhoneNumberViewModel.Phone>(qrCodeCreatorModel, PhoneNumberValidator)
{
    public sealed record class Phone(string PhoneNumber = "")
    {
        public Phone() : this(string.Empty) { }
    }

    private static readonly FormValidator<Phone> PhoneNumberValidator =
        new(focusFieldName: "PhoneNumber", fieldValidators: [Validators.PhoneNumber]);

    [ObservableProperty]
    public partial string PhoneNumber { get; set; } = string.Empty;

    partial void OnPhoneNumberChanged(string value) => this.SubmitPhoneNumber();

    private void SubmitPhoneNumber()
        => this.Submit(value => new QrPhoneNumber(Validators.CleanPhoneNumber(value.PhoneNumber)));
}
