namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class ModulesViewModel : ViewModel<ModulesView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    private int lightLuminance;
    private int darkLuminance;

    public ModulesViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.LightColor = Color.FromUInt32(this.qrCodeCreatorModel.FalseColor);
        this.DarkColor = Color.FromUInt32(this.qrCodeCreatorModel.TrueColor);
        this.LightColorLuminanceValue = string.Empty;
        this.DarkColorLuminanceValue = string.Empty;
        this.ModuleSizeSliderValue = (double)this.qrCodeCreatorModel.Scale;
        this.BorderSizeSliderValue = (double)this.qrCodeCreatorModel.BorderSize;
        this.ModuleSizeString = string.Empty;
        this.BorderSizeString = string.Empty;
    }

    [ObservableProperty]
    public partial double ModuleSizeSliderValue { get; set; }

    [ObservableProperty]
    public partial string ModuleSizeString { get; set; } 

    [ObservableProperty]
    public partial double BorderSizeSliderValue { get; set; }

    [ObservableProperty]
    public partial string BorderSizeString { get; set; } 

    [ObservableProperty]
    public partial Color LightColor { get; set; }

    [ObservableProperty]
    public partial string LightColorLuminanceValue { get; set; }

    [ObservableProperty]
    public partial Color DarkColor { get; set; }

    [ObservableProperty]
    public partial string DarkColorLuminanceValue { get; set; }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.OnLightColorChanged(this.LightColor);
        this.OnDarkColorChanged(this.DarkColor);
        this.OnModuleSizeSliderValueChanged(this.ModuleSizeSliderValue);
        this.OnBorderSizeSliderValueChanged(this.BorderSizeSliderValue);
    }

    [RelayCommand]
    public void OnCopyColors ()
    {
        this.LightColor = Color.FromUInt32(this.qrCodeCreatorModel.FrameForegroundColor);
        this.DarkColor = Color.FromUInt32(this.qrCodeCreatorModel.FrameBackgroundColor);
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

    partial void OnModuleSizeSliderValueChanged(double value)
    {
        int intValue = (int)value;
        if (this.qrCodeCreatorModel.SetScale(intValue))
        {
            this.ModuleSizeString = intValue == 1 ? "One pixel" : string.Format("{0} pixels", intValue);
        } 
    }

    partial void OnBorderSizeSliderValueChanged(double value)
    {
        int intValue = (int)value;
        if (this.qrCodeCreatorModel.SetBorderSize(intValue))
        {
            this.BorderSizeString = intValue == 1 ? "One module" : string.Format("{0} modules", intValue);
        }
    }
}
