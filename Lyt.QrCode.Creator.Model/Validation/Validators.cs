namespace Lyt.QrCode.Creator.Model.Validation;

public static class Validators
{
    public class SecurityCode : AbstractValidator<string>
    {
        public SecurityCode()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("Please enter the security code.")
                .MinimumLength(2).WithMessage("More than one character.")
                .MaximumLength(6).WithMessage("Length should not exceed 6.")
                .Must(x => int.TryParse(x, out int _)).WithMessage("The security code is a number.");
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

    //public class CredentialsMatchingPasswords : AbstractValidator<Credentials>
    //{
    //    public CredentialsMatchingPasswords()
    //    {
    //        this.RuleFor(x => x.Password)
    //            .Equal(x => x.PasswordAgain).WithMessage("Passwords must be identical.");
    //    }
    //}
}