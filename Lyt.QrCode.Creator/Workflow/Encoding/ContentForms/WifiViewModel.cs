namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class WifiViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<WifiView, WifiViewModel.Wifi>(qrCodeCreatorModel, WifiValidator)
{
    public sealed record class Wifi(
        string SsId = "", string Password = "",
        QrWifi.AuthenticationMode Mode = QrWifi.AuthenticationMode.WPA2,
        bool IsHiddenNetwork = true)
    {
        public Wifi() : this(string.Empty, string.Empty) { }
    }

    public class SsIdStringValidator : AbstractValidator<string>
    {
        public SsIdStringValidator()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("The network name (SSID) cannot be empty.")
                .MinimumLength(4).WithMessage("The network name is too short.")
                .MaximumLength(40).WithMessage("The network name is too long.");
        }
    }

    private static readonly FieldValidator<string> SsIdValidator =
        new(new(Validator: new SsIdStringValidator(), SourcePropertyName: "SsId"));

    private static readonly FieldValidator<string> PasswordValidator =
        new(new(Validator: new Validators.AlwaysValid<string>(), SourcePropertyName: "Password"));

    private static readonly FieldValidator<QrWifi.AuthenticationMode> ModeValidator =
        new(new(Validator: new Validators.AlwaysValid<QrWifi.AuthenticationMode>(), SourcePropertyName: "Mode"));

    private static readonly FieldValidator<bool> IsHiddenNetworkValidator =
        new(new(Validator: new Validators.AlwaysValid<bool>(), SourcePropertyName: "IsHiddenNetwork"));

    private static readonly FormValidator<Wifi> WifiValidator =
        new(
            new(
                FocusFieldName: "SsIdTextBox",
                FormValidator: new WifiPasswordValidator(),
                FieldValidators:
                [
                    SsIdValidator,
                    PasswordValidator,
                    ModeValidator,
                    IsHiddenNetworkValidator,
                ]));

    public class WifiPasswordValidator : AbstractValidator<Wifi>
    {
        public WifiPasswordValidator()
        {
            this.When(x => x.Mode != QrWifi.AuthenticationMode.None,
                () =>
                {
                    this.RuleFor(x => x.Password)
                        .NotEmpty().WithMessage("The password cannot be empty when using any authentication mode.")
                        .MinimumLength(8).WithMessage("The password length must be at least 8.")
                        .MaximumLength(40).WithMessage("The password length must not exceed 40.");
                });
        }
    }

    [ObservableProperty]
    public partial string SsId { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial QrWifi.AuthenticationMode Mode { get; set; } = QrWifi.AuthenticationMode.WPA2;

    [ObservableProperty]
    public partial bool IsHiddenNetwork { get; set; } = true;

    partial void OnSsIdChanged(string value) => this.SubmitWifi();

    partial void OnPasswordChanged(string value) => this.SubmitWifi();

    partial void OnModeChanged(QrWifi.AuthenticationMode value) => this.SubmitWifi();

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
