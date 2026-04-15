namespace Lyt.QrCode.Creator.Workflow.Encoding.ContentForms;

public sealed partial class WifiViewModel(QrCodeCreatorModel qrCodeCreatorModel) :
    FormViewModel<WifiView, WifiViewModel.Wifi>(qrCodeCreatorModel, WifiValidator)
{
    public sealed record class Wifi(
        string SsId = "", string Password = "",
        QrWifi.AuthenticationMode Mode = QrWifi.AuthenticationMode.WPA2,
        bool IsHiddenNetwork = true, bool UseWifi_S = true)
    {
        public Wifi() : this(string.Empty, string.Empty) { }
    }

    public static readonly FieldValidator<string> SsIdValidator =
        new(new(
            Validator: new Validators.BasicString(),
            SourcePropertyName: "SsId",
            MessagePropertyName: "ValidationMessage"));

    public static readonly FieldValidator<string> WifiPasswordValidator =
        new(new(
            Validator: new Validators.AlwaysValid<string>(),
            SourcePropertyName: "Password",
            MessagePropertyName: "ValidationMessage"));

    public static readonly FieldValidator<QrWifi.AuthenticationMode> ModeValidator =
        new(new(
            Validator: new Validators.AlwaysValid<QrWifi.AuthenticationMode>(),
            SourcePropertyName: "Mode",
            MessagePropertyName: "ValidationMessage"));

    public static readonly FieldValidator<bool> IsHiddenNetworkValidator =
        new(new(
            Validator: new Validators.AlwaysValid<bool>(),
            SourcePropertyName: "IsHiddenNetwork",
            MessagePropertyName: "ValidationMessage"));

    public static readonly FieldValidator<bool> UseWifiSValidator =
        new(new(
            Validator: new Validators.AlwaysValid<bool>(),
            SourcePropertyName: "IsHiddenNetwork",
            MessagePropertyName: "ValidationMessage"));

    private static readonly FormValidator<Wifi> WifiValidator =
        new(
            new(
                FormValidPropertyName: "FormIsValid",
                MessagePropertyName: "ValidationMessage",
                FocusFieldName: "SsIdTextBox",
                FieldValidators: 
                [
                    SsIdValidator, 
                    WifiPasswordValidator,
                    ModeValidator,
                    IsHiddenNetworkValidator,
                    UseWifiSValidator,
                ]));

    [ObservableProperty]
    public partial string SsId { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial QrWifi.AuthenticationMode Mode { get; set; } = QrWifi.AuthenticationMode.WPA2;

    [ObservableProperty]
    public partial bool IsHiddenNetwork { get; set; } = true;

    [ObservableProperty]
    public partial bool UseWifiS { get; set; } = true;

    partial void OnSsIdChanged(string value) => this.SubmitWifi();

    partial void OnPasswordChanged(string value) => this.SubmitWifi();

    partial void OnModeChanged(QrWifi.AuthenticationMode value) => this.SubmitWifi();

    partial void OnIsHiddenNetworkChanged(bool value) => this.SubmitWifi();

    partial void OnUseWifiSChanged(bool value) => this.SubmitWifi();

    private void SubmitWifi()
        => this.Submit(value =>
        {
            var content = new QrWifi(value.SsId, value.Password, value.Mode, value.IsHiddenNetwork, value.UseWifi_S);
            if (!this.qrCodeCreatorModel.SetContent(content))
            {
                Debug.WriteLine("Failed to set content");
            }
        });
}
