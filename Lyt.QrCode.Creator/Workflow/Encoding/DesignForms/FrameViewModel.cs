namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

using global::Avalonia.Media.Fonts;

public sealed partial class FrameViewModel : ViewModel<FrameView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    private readonly List<int> SupportedFontWeightValues =
    [
        100, 200, 300, 350 ,
        400, 500, 600, 700,
        800, 900, 
        // 950 // Apparently not supported 
    ];

    private readonly List<string> SupportedFontWeightText =
    [
        "Thin - 100",
        "Extra Light - 200",
        "Light - 300",
        "Semi Light - 350",

        "Normal / Regular - 400",
        "Medium - 500",
        "Semi Bold - 600",
        "Bold - 700",

        "Extra Bold - 800",
        "Heavy - 900",
        // "Solid - 950", // 950 // Apparently not supported 
    ];


    public FrameViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.BackgroundColor = Color.FromUInt32(this.qrCodeCreatorModel.FrameBackgroundColor);
        this.ForegroundColor = Color.FromUInt32(this.qrCodeCreatorModel.FrameForegroundColor);
        this.TextTop = this.qrCodeCreatorModel.FrameTextTop;
        this.TextBottom = this.qrCodeCreatorModel.FrameTextBottom;
        this.TextTopFontSize = this.qrCodeCreatorModel.FrameTextTopFontSize.ToString("D");
        this.TextBottomFontSize = this.qrCodeCreatorModel.FrameTextBottomFontSize.ToString("D");
        this.SupportedFontWeights = this.SupportedFontWeightText;

        var fontCollection = FontManager.Current.SystemFonts;
        var fontFamilies = new List<FontFamily>(fontCollection).OrderBy(x => x.Name).ToList();

        // UGLY HACK !
        // Crash when opening the combo if the InterV font is present in the list
        // Note: Inter is doing fine...
        var toRemove =
            (from family in fontFamilies
             where family.Name.StartsWith("InterV", StringComparison.InvariantCultureIgnoreCase)
             // where family.Name.StartsWith("Inter", StringComparison.InvariantCultureIgnoreCase) 
             select family).ToList();
        if (toRemove.Count > 0)
        {
            foreach (var family in toRemove)
            {
                fontFamilies.Remove(family);
            }
        }

        this.SupportedFontFamilies = fontFamilies;

        // Enforce property changed
        this.SelectedTopTextFontWeightsIndex = 0;
        this.SelectedBottomTextFontWeightsIndex = 0;
        this.SelectedTopTextFontWeightsIndex = 6;
        this.SelectedBottomTextFontWeightsIndex = 4;

        this.FrameSizeString = string.Empty;
        this.FrameSizeSliderValue = (double)this.qrCodeCreatorModel.FrameSize;

        this.ValidationMessage = string.Empty;
    }

    [ObservableProperty]
    public partial double FrameSizeSliderValue { get; set; }

    [ObservableProperty]
    public partial string FrameSizeString { get; set; }

    [ObservableProperty]
    public partial Color BackgroundColor { get; set; }

    [ObservableProperty]
    public partial Color ForegroundColor { get; set; }

    [ObservableProperty]
    public partial string TextTop { get; set; }

    [ObservableProperty]
    public partial string TextBottom { get; set; }

    [ObservableProperty]
    public partial string TextTopFontSize { get; set; }

    [ObservableProperty]
    public partial string TextBottomFontSize { get; set; }

    [ObservableProperty]
    public partial List<string> SupportedFontWeights { get; set; }

    [ObservableProperty]
    public partial int SelectedTopTextFontWeightsIndex { get; set; }

    [ObservableProperty]
    public partial int SelectedBottomTextFontWeightsIndex { get; set; }

    [ObservableProperty]
    public partial List<FontFamily> SupportedFontFamilies { get; set; }

    [ObservableProperty]
    public partial int SelectedFontFamilyIndex { get; set; } 

    [ObservableProperty]
    public partial string ValidationMessage { get; set; }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        this.OnBackgroundColorChanged(this.BackgroundColor);
        this.OnForegroundColorChanged(this.ForegroundColor);
        this.OnTextTopChanged(this.TextTop);
        this.OnTextBottomChanged(this.TextBottom);
        this.OnTextTopFontSizeChanged(this.TextTopFontSize);
        this.OnTextBottomFontSizeChanged(this.TextBottomFontSize);
        this.OnFrameSizeSliderValueChanged(this.FrameSizeSliderValue);

        string fontFamilyName = this.qrCodeCreatorModel.FrameTextFontFamily;
        for (int index = 0; index < this.SupportedFontFamilies.Count; ++index )
        {
            if (this.SupportedFontFamilies[index].Name.Equals(fontFamilyName, StringComparison.InvariantCultureIgnoreCase))
            {
                this.SelectedFontFamilyIndex = index;
                this.OnSelectedFontFamilyIndexChanged(index);
                break; 
            }
        }
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
        this.BackgroundColor = Color.FromUInt32(this.qrCodeCreatorModel.FalseColor);
        this.ForegroundColor = Color.FromUInt32(this.qrCodeCreatorModel.TrueColor);
    }

    partial void OnFrameSizeSliderValueChanged(double value)
    {
        int intValue = (int)value;
        this.qrCodeCreatorModel.SetFrameSize(intValue);
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

    partial void OnTextTopFontSizeChanged(string value)
    {
        string trimmedValue = value.Trim();
        if (string.IsNullOrEmpty(trimmedValue))
        {
            return;
        } 

        if ( !int.TryParse(trimmedValue, out int fontSize))
        {
            this.ValidationMessage = "Top text font size must be a valid integer.";
            return;
        }

        if (fontSize < 8 || fontSize > 120)
        {
            this.ValidationMessage = "Top text font size must be between 8 and 120.";
            return;
        }

        this.ValidationMessage = string.Empty   ;
        this.qrCodeCreatorModel.SetFrameTextTopFontSize(fontSize);
    }

    partial void OnTextBottomFontSizeChanged(string value)
    {
        string trimmedValue = value.Trim();
        if (string.IsNullOrEmpty(trimmedValue))
        {
            return;
        }

        if (!int.TryParse(trimmedValue, out int fontSize))
        {
            this.ValidationMessage = "Bottom text font size must be a valid integer.";
            return;
        }

        if (fontSize < 8 || fontSize > 120)
        {
            this.ValidationMessage = "Bottom text font size must be between 8 and 120.";
            return;
        }

        this.ValidationMessage = string.Empty;
        this.qrCodeCreatorModel.SetFrameTextBottomFontSize(fontSize);
    }

    partial void OnSelectedTopTextFontWeightsIndexChanged(int value)
    {
        int weight = this.SupportedFontWeightValues[value];
        this.qrCodeCreatorModel.SetFrameTextTopFontWeight(weight);
    }

    partial void OnSelectedBottomTextFontWeightsIndexChanged(int value)
    {
        int weight = this.SupportedFontWeightValues[value];
        this.qrCodeCreatorModel.SetFrameTextBottomFontWeight(weight);
    }

    partial void OnSelectedFontFamilyIndexChanged(int value)
    {
        var fontFamily = this.SupportedFontFamilies[value];
        this.qrCodeCreatorModel.SetFrameTextFontFamily(fontFamily.Name);
    }
}
