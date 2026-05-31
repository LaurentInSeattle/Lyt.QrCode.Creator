namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class ImageViewModel : ViewModel<ImageView>, IDropImageTarget
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public ImageViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.DropViewModel = new DropViewModel(this);
        this.ColoringSliderValue = 100.0 * this.qrCodeCreatorModel.Coloring;
        this.OpacitySliderValue = this.qrCodeCreatorModel.DarkModulesOpacity;
    }

    [ObservableProperty]
    public partial DropViewModel DropViewModel { get; set; }

    [ObservableProperty]
    public partial double ColoringSliderValue { get; set; }

    [ObservableProperty]
    public partial string ColoringString { get; set; } = string.Empty;

    [ObservableProperty]
    public partial double OpacitySliderValue { get; set; }

    [ObservableProperty]
    public partial string OpacityString { get; set; } = string.Empty;

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.qrCodeCreatorModel.DoUseBackground();
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.OnColoringSliderValueChanged(this.ColoringSliderValue);
        this.OnOpacitySliderValueChanged(this.OpacitySliderValue);
    }

    public override void Deactivate() => this.qrCodeCreatorModel.DoUseBackground(false);

    public void OnImageDrop(byte[] imageBytes) => this.qrCodeCreatorModel.SetBackground(imageBytes);

    partial void OnColoringSliderValueChanged(double value)
    {
        int intValue = (int)(value + 0.5);
        this.qrCodeCreatorModel.SetColoring(value / 100.0);
        this.ColoringString = string.Format("{0} %", intValue);
    }

    partial void OnOpacitySliderValueChanged(double value)
    {
        int intValue = (int)(value * 100 + 0.5);
        this.qrCodeCreatorModel.SetDarkModulesOpacity(value);
        this.OpacityString = string.Format("{0} %", intValue);
    }
}
