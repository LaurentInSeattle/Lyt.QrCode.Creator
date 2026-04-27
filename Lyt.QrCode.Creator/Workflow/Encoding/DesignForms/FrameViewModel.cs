namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class FrameViewModel : ViewModel<FrameView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public FrameViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.BackgroundColor = Color.FromUInt32(this.qrCodeCreatorModel.FrameBackgroundColor);
        this.ForegroundColor = Color.FromUInt32(this.qrCodeCreatorModel.FrameForegroundColor);
        this.TextTop = this.qrCodeCreatorModel.FrameTextTop;
        this.TextBottom = this.qrCodeCreatorModel.FrameTextBottom;
        this.FrameSizeSliderValue = (double) this.qrCodeCreatorModel.FrameSize;
    }

    [ObservableProperty]
    public partial double FrameSizeSliderValue { get; set; } = 6;

    [ObservableProperty]
    public partial string FrameSizeString { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Color BackgroundColor { get; set; }

    [ObservableProperty]
    public partial Color ForegroundColor { get; set; }

    [ObservableProperty]
    public partial string TextTop { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TextBottom{ get; set; } = string.Empty;

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        this.OnBackgroundColorChanged(this.BackgroundColor);
        this.OnForegroundColorChanged(this.ForegroundColor);
        this.OnTextTopChanged(this.TextTop);
        this.OnTextBottomChanged(this.TextBottom);
        this.OnFrameSizeSliderValueChanged(this.FrameSizeSliderValue); 
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.qrCodeCreatorModel.DoUseFrame(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.qrCodeCreatorModel.DoUseFrame(false);
    }

    [RelayCommand]
    public void OnCopyColors()
    {
        this.BackgroundColor= Color.FromUInt32(this.qrCodeCreatorModel.FalseColor);
        this.ForegroundColor = Color.FromUInt32(this.qrCodeCreatorModel.TrueColor);
    }

    partial void OnFrameSizeSliderValueChanged(double value)
    {
        int intValue = (int)value;
        this.qrCodeCreatorModel.SetFrameSize (intValue);
        this.FrameSizeString = intValue == 1 ? "One module" : string.Format("{0} modules", intValue);
    }

    partial void OnForegroundColorChanged(Color value) 
        => this.qrCodeCreatorModel.SetFrameForegroundColor(value.ToUInt32());

    partial void OnBackgroundColorChanged(Color value)
        => this.qrCodeCreatorModel.SetFrameBackgroundColor(value.ToUInt32());

    partial void OnTextTopChanged(string value) 
        => this.qrCodeCreatorModel.SetFrameTextTop(value);

    partial void OnTextBottomChanged(string value) 
        => this.qrCodeCreatorModel.SetFrameTextBottom(value);   
}
