namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class ColorsViewModel : ViewModel<ColorsView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    [ObservableProperty]
    public partial Color LightColor { get; set; }

    [ObservableProperty]
    public partial string LightColorLuminanceValue { get; set; }

    [ObservableProperty]
    public partial Color DarkColor { get; set; }

    [ObservableProperty]
    public partial string DarkColorLuminanceValue { get; set; }

    private int lightLuminance;
    private int darkLuminance;

    public ColorsViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.LightColor = Colors.White;
        this.DarkColor = Colors.Black;
        this.LightColorLuminanceValue = string.Empty;
        this.DarkColorLuminanceValue = string.Empty;
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.OnLightColorChanged(Colors.White);
        this.OnDarkColorChanged(Colors.Black);
    }

    partial void OnLightColorChanged(Color value)
    {
        double luminance =
            value.R * 0.30 +
            value.G * 0.59 +
            value.B * 0.11 ;
        luminance /= 255;
        luminance *= 100;
        this.lightLuminance = (int)luminance;
        if (this.lightLuminance < 75)
        {
            this.LightColorLuminanceValue = "Too Dark";
            return;
        }

        this.LightColorLuminanceValue = string.Format("Luminance: {0} %" , this.lightLuminance);
        this.qrCodeCreatorModel.SetLightColor(value.ToUInt32());
    }

    partial void OnDarkColorChanged(Color value)
    {
        double luminance =
            value.R * 0.30 +
            value.G * 0.59 +
            value.B * 0.11;
        luminance /= 255;
        luminance *= 100;
        this.darkLuminance = (int)luminance;
        if (this.darkLuminance > 25)
        {
            this.DarkColorLuminanceValue = "Too Light";
            return;
        }

        this.DarkColorLuminanceValue = string.Format("Luminance: {0} %", this.darkLuminance);
        this.qrCodeCreatorModel.SetDarkColor(value.ToUInt32());
    } 
}
