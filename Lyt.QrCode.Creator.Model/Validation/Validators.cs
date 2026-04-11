namespace Lyt.QrCode.Creator.Model.Validation;

public static class Validators
{
    public static readonly FieldValidatorParameters<string> TitleValidatorParameters =
        new(
            Validator: new Validators.BasicString(),
            SourcePropertyName: "Title",
            MessagePropertyName: "ValidationMessage");

    public static readonly FieldValidator<string> TitleValidator =
        new(TitleValidatorParameters);

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

    public class Email : AbstractValidator<string>
    {
        public Email()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("Your email cannot be empty")
                .EmailAddress().WithMessage("This email is invalid or malformed.");
        }
    }

    //public class Password : AbstractValidator<string>
    //{
    //    public Password()
    //    {
    //        this.RuleFor(x => x)
    //            .NotEmpty().WithMessage("Your password cannot be empty")
    //            .MinimumLength(10).WithMessage("Your password length must be at least 10.")
    //            .MaximumLength(20).WithMessage("Your password length must not exceed 20.")
    //            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
    //            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
    //    }
    //}

    //public class CredentialsMatchingPasswords : AbstractValidator<Credentials>
    //{
    //    public CredentialsMatchingPasswords()
    //    {
    //        this.RuleFor(x => x.Password)
    //            .Equal(x => x.PasswordAgain).WithMessage("Passwords must be identical.");
    //    }
    //}
}