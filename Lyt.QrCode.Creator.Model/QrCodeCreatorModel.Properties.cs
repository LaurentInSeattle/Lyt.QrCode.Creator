namespace Lyt.QrCode.Creator.Model;

public sealed partial class QrCodeCreatorModel : ModelBase
{
    private const uint ColorBlack = 0xFF_000000;
    private const uint ColorWhite = 0xFF_FFFFFF;

    #region Serialized -  No model changed event

    [JsonRequired]
    public string Language { get => this.Get<string>()!; set => this.Set(value); }

    /// <summary> This should stay true, ==> But... Just FOR NOW !  </summary>
    [JsonRequired]
    public bool IsFirstRun { get; set; } = false;

    #endregion Serialized -  No model changed event

    #region Not serialized - No model changed event

    [JsonIgnore]
    public bool IsUpdatePending { get; set; } = false;

    // Image : NOT as a bitmap to avoid dependency issues in the model 
    [JsonIgnore]
    public object? QrCodeImage { get; set; } = null;

    // Content

    [JsonIgnore]
    public Type QrCodeContentType { get; set; } = typeof(string);

    [JsonIgnore]
    public QrContent QrCodeContent { get; set; } = new();

    [JsonIgnore]
    public bool[,] Modules { get; set; } = new bool[0, 0];

    // Frame 

    // LATER 
    //[JsonIgnore]
    //[AutoNotifyProperty(2)]
    //public partial int FrameSize { get; set; }

    [JsonIgnore]
    public bool UseFrame { get; set; } = false;

    [JsonIgnore]
    public int FrameSize { get; set; } = 6;

    [JsonIgnore]
    public uint FrameForegroundColor { get; set; } = ColorWhite;

    [JsonIgnore]
    public uint FrameBackgroundColor { get; set; } = ColorBlack;

    [JsonIgnore]
    public bool WithRoundedCorners { get; set; } = true;

    [JsonIgnore]
    public string FrameTextTop { get; set; } = "Scan Me!";

    [JsonIgnore]
    public string FrameTextBottom { get; set; } = "to open this link in your browser...";

    [JsonIgnore]
    public string FrameTextFontFamily { get; set; } = "Arial";

    [JsonIgnore]
    public int FrameTextTopFontSize { get; set; } = 24;

    [JsonIgnore]
    public int FrameTextBottomFontSize { get; set; } = 18;

    [JsonIgnore]
    public int FrameTextTopFontWeight { get; set; } = 500;

    [JsonIgnore]
    public int FrameTextBottomFontWeight { get; set; } = 400;

    // Module colors, Scale and Border 

    [JsonIgnore]
    public int Scale { get; set; } = 16;

    [JsonIgnore]
    public int BorderSize { get; set; } = 2;

    [JsonIgnore]
    public uint FalseColor { get; set; } = ColorWhite;

    [JsonIgnore]
    public uint TrueColor { get; set; } = ColorBlack;

    // Logo

    [JsonIgnore]
    public bool UseLogo { get; set; } = false;

    [JsonIgnore]
    public double LogoSize { get; set; } = 0.25;

    [JsonIgnore]
    public int LogoQuietZone { get; set; } = 1;

    [JsonIgnore]
    public byte[] LogoImageBytes { get; set; } = [];

    // Background

    [JsonIgnore]
    public bool UseBackground { get; set; } = false;

    [JsonIgnore]
    public double Coloring { get; set; } = 0.5;

    [JsonIgnore]
    public double DarkModulesOpacity { get; set; } = 0.75;

    [JsonIgnore]
    public byte[] BackgroundImageBytes { get; set; } = [];

    // Finders and Module shapes

    [JsonIgnore]
    public ModuleShape ModuleShape { get; set; } = ModuleShape.Square;

    [JsonIgnore]
    public FinderShape FinderShape { get; set; } = FinderShape.Square;

    // Output Format 

    [JsonIgnore]
    public OutputLocation OutputLocation { get; set; } = OutputLocation.Desktop;

    [JsonIgnore]
    public OutputFormat OutputFormat { get; set; } = OutputFormat.Png;

    [JsonIgnore]
    public string OutputFileName { get; set; } = "QR-Code";

    [JsonIgnore]
    public bool UseTimeStamp { get; set; } = true;

    public string OutputFilePath()
    {
        string rootPath = this.OutputLocation.FolderPath();
        string fileName = this.OutputFileName;
        if (this.UseTimeStamp)
        {
            fileName += $"_{FileManagerModel.TimestampString()}";
        }   

        string extension = this.OutputFormat.FileExtension();
        return Path.Combine(rootPath, $"{fileName}.{extension}");
    }

    #endregion Not serialized - No model changed event

    #region NOT serialized - WITH model changed event

    // None for now 

    #endregion NOT serialized - WITH model changed event    
}
