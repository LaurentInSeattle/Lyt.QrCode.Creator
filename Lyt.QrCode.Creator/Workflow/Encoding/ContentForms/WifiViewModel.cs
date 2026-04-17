namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

using static Lyt.QrCode.Creator.Model.Validation.Validators;
using static Lyt.QrCode.Content.QrWifi;

public sealed partial class WifiViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<WifiView, WifiViewModel.Wifi>(qrCodeCreatorModel, WifiValidator)
{
    public sealed record class Wifi(
        string SsId = "", 
        string Password = "",
        QrWifi.AuthenticationMode Mode = QrWifi.AuthenticationMode.WPA2,
        bool IsHiddenNetwork = true)
    {
        public Wifi() : this(string.Empty, string.Empty) { }
    }

    public class SsIdStringValidator : AbstractValidator<string>
    {
        public SsIdStringValidator()
            => this.RuleFor(x => x)
                .NotEmpty().WithMessage("The network name (SSID) cannot be empty.")
                .MinimumLength(4).WithMessage("The network name is too short.")
                .MaximumLength(40).WithMessage("The network name is too long.");
    }

    private static readonly FormValidator<Wifi> WifiValidator =
        new(focusFieldName: "SsIdTextBox",
            formValidator: new WifiPasswordValidator(),
            fieldValidators:
            [
                new FieldValidator<string>("SsId", new SsIdStringValidator()),
                AlwaysValid<string>("Password"),
                AlwaysValid<AuthenticationMode>("Mode"),
                AlwaysValid<bool>("IsHiddenNetwork"),
            ]);

    public class WifiPasswordValidator : AbstractValidator<Wifi>
    {
        public WifiPasswordValidator()
            => this.When(x => x.Mode != AuthenticationMode.None,
                () =>
                {
                    this.RuleFor(x => x.Password)
                        .NotEmpty().WithMessage("The password cannot be empty when using any authentication mode.")
                        .MinimumLength(8).WithMessage("The password length must be at least 8.")
                        .MaximumLength(40).WithMessage("The password length must not exceed 40.");
                });
    }

    [ObservableProperty]
    public partial string SsId { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial AuthenticationMode Mode { get; set; } = AuthenticationMode.WPA2;

    [ObservableProperty]
    public partial bool IsHiddenNetwork { get; set; } = true;

    partial void OnSsIdChanged(string value) => this.SubmitWifi();

    partial void OnPasswordChanged(string value) => this.SubmitWifi();

    partial void OnModeChanged(AuthenticationMode value) => this.SubmitWifi();

    partial void OnIsHiddenNetworkChanged(bool value) => this.SubmitWifi();

    private void SubmitWifi()
        => this.Submit(value =>
        {
            var content = new QrWifi(value.SsId, value.Password, value.Mode, value.IsHiddenNetwork, false);
            if (!this.qrCodeCreatorModel.SetContent(content))
            {
                Debug.WriteLine("Failed to set content");
            }
        });
}
