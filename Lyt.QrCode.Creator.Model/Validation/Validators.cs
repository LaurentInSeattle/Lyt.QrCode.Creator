namespace Lyt.QrCode.Creator.Model.Validation;

public static class Validators
{
    public static FieldValidator<T> AlwaysValid<T>(string sourcePropertyName)
        => new(sourcePropertyName, allowEmpty:true, validator: new AlwaysValidValidator<T>());

    public class AlwaysValidValidator<T> : AbstractValidator<T> { }

    //public static FieldValidator<TValidator> Optional<TValidator>(string sourcePropertyName)
    //    where TValidator : AbstractValidator<string>, new()
    //    => new(validator: new Validators.OptionalValidator<TValidator>(), sourcePropertyName: sourcePropertyName);

    //public class  OptionalValidator<TValidator> : AbstractValidator<string> 
    //    where TValidator : AbstractValidator<string>, new()
    //{
    //    public OptionalValidator()
    //    {
    //        this.When(x => string.IsNullOrEmpty(x), () => { });

    //        this.When(x => !string.IsNullOrEmpty(x), () => { new TValidator(); });
    //    }
    //}

    public class BasicString : AbstractValidator<string>
    {
        public BasicString()
            => this.RuleFor(x => x)
                .NotEmpty().WithMessage("Cannot be empty.")
                .MinimumLength(2).WithMessage("Too short.")
                .MaximumLength(80).WithMessage("Too long.");
    }

    public static readonly FieldValidator<string> TitleValidator = new("Title", validator: new BasicString());

    public static readonly FieldValidator<string> UrlValidator = new("Url", validator: new Url());

    public class Url : AbstractValidator<string>
    {
        public Url()
            => this.RuleFor(x => x)
                .NotEmpty().WithMessage("The Url cannot be empty.")
                .MinimumLength(8).WithMessage("Too short.")
                .MaximumLength(420).WithMessage("Too long.")
                .Must(x => x.StartsWith("http://") || x.StartsWith("https://"))
                .WithMessage("The Url does not begin with a valid Web http or https protocol.");

    }

    public class LatitudeString : AbstractValidator<string>
    {
        public LatitudeString()
            => this.RuleFor(x => x)
                .NotEmpty().WithMessage("Latitude field cannot be empty.")
                .MinimumLength(1).WithMessage("Too short.")
                .MaximumLength(40).WithMessage("Too long.")
                .Must(x =>
                    double.TryParse(x, out double value) &&
                    !double.IsNaN(value) &&
                    double.IsFinite(value)).WithMessage("Latitude must be a valid number.")
                .Must(x =>
                    double.TryParse(x, out double value) &&
                    value >= -90.0 && value <= 90.0).WithMessage("Latitude must be between -90.0 and 90.0 degrees.");
    }

    public static readonly FieldValidator<string> Latitude = new("Latitude", validator: new LatitudeString());

    public class LongitudeString : AbstractValidator<string>
    {
        public LongitudeString()
            => this.RuleFor(x => x)
                .NotEmpty().WithMessage("Longitude field cannot be empty.")
                .MinimumLength(1).WithMessage("Too short.")
                .MaximumLength(40).WithMessage("Too long.")
                .Must(x =>
                    double.TryParse(x, out double value) &&
                    !double.IsNaN(value) &&
                    double.IsFinite(value)).WithMessage("Longitude must be a valid number.")
                .Must(x =>
                    double.TryParse(x, out double value) &&
                    value >= -180.0 && value <= 180.0).WithMessage("Longitude must be between -180.0 and 180.0 degrees");
    }

    public static readonly FieldValidator<string> Longitude = new("Longitude", validator: new LongitudeString());

    public class Email : AbstractValidator<string>
    {
        public Email()
            => this.RuleFor(x => x)
                .NotEmpty().WithMessage("This email address cannot be empty")
                .EmailAddress().WithMessage("This email address is invalid or malformed.");
    }

    public static readonly FieldValidator<string> EmailAddress = new("EmailAddress", validator: new Email());

    public static string CleanPhoneNumber(string x)
    {
        string y = x.Replace("(", string.Empty);
        y = y.Replace(")", string.Empty);
        y = y.Replace("-", string.Empty);
        y = y.Replace(" ", string.Empty);
        return y.Trim();
    }

    public class Phone : AbstractValidator<string>
    {
        public Phone()        
            => this.RuleFor(x => CleanPhoneNumber(x))
                .NotEmpty().WithMessage("This phone number cannot be empty")
                .MinimumLength(4).WithMessage("This phone number is too short.")
                .MaximumLength(20).WithMessage("This phone number is too long.")
                .Matches(@"^\+?[1-9][0-9]{7,14}$").WithMessage("This phone number is invalid or malformed.");        
    }

    public static readonly FieldValidator<string> PhoneNumber = 
        new("PhoneNumber", validator: new Phone());

    public static readonly FieldValidator<string> OptionalPhoneNumber = 
        new("PhoneNumber", allowEmpty:true, validator: new Phone());
}