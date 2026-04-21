namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

using static Lyt.QrCode.Content.QrVCard;
using static Lyt.QrCode.Creator.Model.Validation.Validators;

public partial class VCardViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<VCardView, VCardViewModel.VCard>(qrCodeCreatorModel, VCardValidator)
{
    public sealed record class VCard(
        string FirstName = "",
        string LastName = "",
        string Title = "",
        string Nickname = "",
        string FullName = "",
        string Note = "",
        string Organization = "",
        string PrimaryPhone = "",
        string MobilePhone = "",
        string WorkPhone = "",
        string Email = "",
        string Website = "",
        ContactAddressFormat Format = ContactAddressFormat.European,
        AddressKind Kind = AddressKind.Home,
        string Street = "",
        string HouseNumber = "",
        string City = "",
        string ZipCode = "",
        string StateRegion = "",
        string Country = "")
    {
        public VCard() :
            this(string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty,
                ContactAddressFormat.European,
                AddressKind.Home,
                string.Empty, 
                string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty)
        { }
    }

    [ObservableProperty]
    public partial string FirstName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string LastName { get; set; } = string.Empty;

    // VCARD ONLY  
    [ObservableProperty]
    public partial AddressKind Kind { get; set; }

    [ObservableProperty]
    public partial string FullName { get; set; } = string.Empty;

    // BOTH Optional 
    [ObservableProperty]
    public partial ContactAddressFormat Format { get; set; } = ContactAddressFormat.NorthAmerica;

    // All other relevant optional Card fields as properties defaulting to empty 
    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Nickname { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Organization { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Note { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PrimaryPhone { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string MobilePhone { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WorkPhone { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Email { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Website { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string HouseNumber { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Street { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string City { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ZipCode { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string StateRegion { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Country { get; set; } = string.Empty;

   
    public class FirstNameValidator : AbstractValidator<string>
    {
        public FirstNameValidator()
            => this.RuleFor(x => x)
                .NotEmpty().WithMessage("'First Name' cannot be empty.")
                .MinimumLength(2).WithMessage("The first name is too short.")
                .MaximumLength(60).WithMessage("The first name is too long.");
    }

    public class LastNameValidator : AbstractValidator<string>
    {
        public LastNameValidator()
            => this.RuleFor(x => x)
                .NotEmpty().WithMessage("'Last Name' cannot be empty.")
                .MinimumLength(2).WithMessage("The last name is too short.")
                .MaximumLength(60).WithMessage("The last name is too long.");
    }

    private static readonly FormValidator<VCard> VCardValidator =
        new(focusFieldName: "FirstNameTextBox",
            fieldValidators:
            [
                new FieldValidator<string> ("FirstName", new FirstNameValidator()),
                new FieldValidator<string> ("LastName", new LastNameValidator()),
                AlwaysValid<string>("Title"),
                AlwaysValid<string>("Nickname"),
                AlwaysValid<string>("FullName"),
                new FieldValidator<string> ("PrimaryPhone", allowEmpty:true, validator: new Phone() ),
                new FieldValidator<string> ("MobilePhone", allowEmpty:true, validator: new Phone() ),
                new FieldValidator<string> ("WorkPhone", allowEmpty:true, validator: new Phone() ),
                new FieldValidator<string> ("Email", allowEmpty:true, validator: new Email() ),
                AlwaysValid<string>("Organization"),
                new FieldValidator<string> ("Website", allowEmpty:true, validator: new Url() ),
                AlwaysValid<string>("Note"),

                // Address fields: We need those so that data is going to be copied in the 
                // final validated object, but we don't want to validate them as they are optional
                // and have no specific format.
                AlwaysValid<string>("Street"),
                AlwaysValid<string>("HouseNumber"),
                AlwaysValid<string>("City"),
                AlwaysValid<string>("ZipCode"),
                AlwaysValid<string>("StateRegion"),
                AlwaysValid<string>("Country"),
                AlwaysValid<AddressKind>("Kind"),
                AlwaysValid<ContactAddressFormat>("Format")
            ]);

    partial void OnFirstNameChanged(string value) => this.SubmitVCard();
    partial void OnLastNameChanged(string value) => this.SubmitVCard();
    partial void OnTitleChanged(string value) => this.SubmitVCard();
    partial void OnNicknameChanged(string value) => this.SubmitVCard();
    partial void OnFullNameChanged(string value) => this.SubmitVCard();

    partial void OnOrganizationChanged(string value) => this.SubmitVCard();
    partial void OnPrimaryPhoneChanged(string value) => this.SubmitVCard();
    partial void OnMobilePhoneChanged(string value) => this.SubmitVCard();
    partial void OnWorkPhoneChanged(string value) => this.SubmitVCard();
    partial void OnEmailChanged(string value) => this.SubmitVCard();
    partial void OnWebsiteChanged(string value) => this.SubmitVCard();
    partial void OnNoteChanged(string value) => this.SubmitVCard();

    partial void OnFormatChanged(ContactAddressFormat value) => this.SubmitVCard();
    partial void OnKindChanged(AddressKind value) => this.SubmitVCard();

    partial void OnStreetChanged(string value) => this.SubmitVCard();
    partial void OnHouseNumberChanged(string value) => this.SubmitVCard();
    partial void OnCityChanged(string value) => this.SubmitVCard();
    partial void OnZipCodeChanged(string value) => this.SubmitVCard();
    partial void OnStateRegionChanged(string value) => this.SubmitVCard();
    partial void OnCountryChanged(string value) => this.SubmitVCard();

    public override void OnViewLoaded()
        => Schedule.OnUiThread(160, this.ForceRadioButton, DispatcherPriority.ApplicationIdle);

    private void ForceRadioButton() => this.View.AddressKindButton.IsChecked = true;

    private void SubmitVCard()
        => this.Submit(value =>
        {
            return
                new QrVCard(value.FirstName, value.LastName)
                {
                    Fullname = value.FullName,
                    Title = value.Title,
                    Nickname = value.Nickname,
                    Note = value.Note,
                    Organization = value.Organization,
                    Phone = value.PrimaryPhone,
                    MobilePhone = value.MobilePhone,
                    WorkPhone = value.WorkPhone,
                    Email = value.Email,
                    Website = value.Website,
                    Kind = value.Kind,
                    Format = value.Format,
                    Street = value.Street,
                    HouseNumber = value.HouseNumber,
                    City = value.City,
                    ZipCode = value.ZipCode,
                    StateRegion = value.StateRegion,
                    Country = value.Country,
                };
        });
}
