namespace Lyt.QrCode.Creator.Model.Validation;

public static class Validators
{
    public class AlwaysValid<T> : AbstractValidator<T> { }

    public class BasicString : AbstractValidator<string>
    {
        public BasicString()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("Cannot be empty.")
                .MinimumLength(2).WithMessage("Too short.")
                .MaximumLength(80).WithMessage("Too long.");
        }
    }

    public static readonly FieldValidatorParameters<string> TitleValidatorParameters =
        new(
            Validator: new Validators.BasicString(),
            SourcePropertyName: "Title",
            MessagePropertyName: "ValidationMessage");

    public static readonly FieldValidator<string> TitleValidator =
        new(TitleValidatorParameters);

    public static readonly FieldValidatorParameters<string> UrlValidatorParameters =
        new(
            Validator: new Validators.Url(),
            SourcePropertyName: "Url",
            MessagePropertyName: "ValidationMessage");

    public static readonly FieldValidator<string> UrlValidator =
        new(UrlValidatorParameters);

    public class Url : AbstractValidator<string>
    {
        public Url()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("The Url cannot be empty.")
                .MinimumLength(8).WithMessage("Too short.")
                .MaximumLength(420).WithMessage("Too long.")
                .Must(x => x.StartsWith("http://") || x.StartsWith("https://"))
                    .WithMessage("The Url does not begin with a valid Web http or https protocol.");
        }
    }

    public class LatitudeString : AbstractValidator<string>
    {
        public LatitudeString()
        {
            this.RuleFor(x => x)
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
    }

    public static readonly FieldValidatorParameters<string> LatitudeValidatorParameters =
        new(
            Validator: new Validators.LatitudeString(),
            SourcePropertyName: "Latitude",
            MessagePropertyName: "ValidationMessage");

    public static readonly FieldValidator<string> Latitude =
        new(LatitudeValidatorParameters);

    public class LongitudeString : AbstractValidator<string>
    {
        public LongitudeString()
        {
            this.RuleFor(x => x)
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
    }

    public static readonly FieldValidatorParameters<string> LongitudeValidatorParameters =
        new(
            Validator: new Validators.LongitudeString(),
            SourcePropertyName: "Longitude",
            MessagePropertyName: "ValidationMessage");

    public static readonly FieldValidator<string> Longitude =
        new(LongitudeValidatorParameters);

    public class Email : AbstractValidator<string>
    {
        public Email()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("This email address cannot be empty")
                .EmailAddress().WithMessage("This email address is invalid or malformed.");
        }
    }

    public static readonly FieldValidator<string> EmailAddress =
        new(new(
            Validator: new Validators.Email(),
            SourcePropertyName: "EmailAddress",
            MessagePropertyName: "ValidationMessage"));

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
        {

            this.RuleFor(x => CleanPhoneNumber(x))
                .NotEmpty().WithMessage("This phone number cannot be empty")
                .MinimumLength(4).WithMessage("Too short.")
                .MaximumLength(40).WithMessage("Too long.")
                .Matches( @"^\+?[1-9][0-9]{7,14}$") .WithMessage("This phone number is invalid or malformed.");
        }
    }

    public static readonly FieldValidator<string> PhoneNumber =
        new(new(
            Validator: new Validators.Phone(),
            SourcePropertyName: "PhoneNumber",
            MessagePropertyName: "ValidationMessage"));

    public class Password : AbstractValidator<string>
    {
        public Password()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("Your password cannot be empty")
                .MinimumLength(10).WithMessage("Your password length must be at least 10.")
                .MaximumLength(20).WithMessage("Your password length must not exceed 20.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
        }
    }
}