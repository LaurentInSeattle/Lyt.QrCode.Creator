namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

using static Lyt.QrCode.Creator.Model.Validation.Validators;

public sealed partial class MeCardViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<MeCardView, MeCardViewModel.MeCard>(qrCodeCreatorModel, MeCardValidator)
{
    public sealed record class MeCard(
        string FirstName = "",
        string LastName = "",
        string Title = "",
        string Nickname = "",
        string Note = "",
        string Organization = "",
        string PrimaryPhone = "",
        string MobilePhone = "",
        string WorkPhone = "",
        string Email = "",
        string Website = "",
        ContactAddressFormat Format = ContactAddressFormat.European,
        string Street = "",
        string PoBox = "",
        string RoomNumber = "",
        string HouseNumber = "",
        string City = "",
        string ZipCode = "",
        string StateRegion = "",
        string Country = "")
    {
        public MeCard() :
            this(string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty,
                ContactAddressFormat.European,
                string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty)
        { }
    }

    [ObservableProperty]
    public partial string FirstName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string LastName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ContactAddressFormat Format { get; set; } = ContactAddressFormat.European;

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
    public partial string PoBox { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string RoomNumber { get; set; } = string.Empty;

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

    private static readonly FormValidator<MeCard> MeCardValidator =
        new(focusFieldName: "FirstNameTextBox",
            fieldValidators:
            [
                new FieldValidator<string> ("FirstName", new FirstNameValidator()),
                new FieldValidator<string> ("LastName", new LastNameValidator()),
                AlwaysValid<string>("Title"),
                AlwaysValid<string>("Nickname"),
                new FieldValidator<string> ("PrimaryPhone", allowEmpty:true, validator: new Phone() ),
                new FieldValidator<string> ("MobilePhone", allowEmpty:true, validator: new Phone() ),
                new FieldValidator<string> ("WorkPhone", allowEmpty:true, validator: new Phone() ),
                new FieldValidator<string> ("Email", allowEmpty:true, validator: new Email() ),
                AlwaysValid<string>("Organization"),
                new FieldValidator<string> ("Website", allowEmpty:true, validator: new Url() ),
            ]);

    partial void OnFirstNameChanged(string value) => this.SubmitMeCard();
    partial void OnLastNameChanged(string value) => this.SubmitMeCard();
    partial void OnTitleChanged(string value) => this.SubmitMeCard();
    partial void OnNicknameChanged(string value) => this.SubmitMeCard();

    partial void OnNoteChanged(string value) => this.SubmitMeCard();
    partial void OnOrganizationChanged(string value) => this.SubmitMeCard();
    partial void OnPrimaryPhoneChanged(string value) => this.SubmitMeCard();
    partial void OnMobilePhoneChanged(string value) => this.SubmitMeCard();
    partial void OnWorkPhoneChanged(string value) => this.SubmitMeCard();
    partial void OnEmailChanged(string value) => this.SubmitMeCard();
    partial void OnWebsiteChanged(string value) => this.SubmitMeCard();

    partial void OnFormatChanged(ContactAddressFormat value) => this.SubmitMeCard();
    partial void OnStreetChanged(string value) => this.SubmitMeCard();
    partial void OnPoBoxChanged(string value) => this.SubmitMeCard();
    partial void OnRoomNumberChanged(string value) => this.SubmitMeCard();
    partial void OnHouseNumberChanged(string value) => this.SubmitMeCard();
    partial void OnCityChanged(string value) => this.SubmitMeCard();
    partial void OnZipCodeChanged(string value) => this.SubmitMeCard();
    partial void OnStateRegionChanged(string value) => this.SubmitMeCard();
    partial void OnCountryChanged(string value) => this.SubmitMeCard();

    private void SubmitMeCard()
        => this.Submit(value =>
        {
            return
                new QrMeCard(value.FirstName, value.LastName)
                {
                    Title = value.Title,
                    Nickname = value.Nickname,
                    Note = value.Note,
                    Organization = value.Organization,
                    Phone = value.PrimaryPhone,
                    MobilePhone = value.MobilePhone,
                    WorkPhone = value.WorkPhone,
                    Email = value.Email,
                    Website = value.Website,
                    Format = value.Format,
                    Street = value.Street,
                    PoBox = value.PoBox,
                    RoomNumber = value.RoomNumber,
                    HouseNumber = value.HouseNumber,
                    City = value.City,
                    ZipCode = value.ZipCode,
                    StateRegion = value.StateRegion,
                    Country = value.Country,
                };
        });
}
