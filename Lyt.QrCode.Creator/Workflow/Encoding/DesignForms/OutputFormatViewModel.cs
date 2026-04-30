namespace Lyt.QrCode.Creator.Workflow.Encoding.DesignForms;

public sealed partial class OutputFormatViewModel : ViewModel<OutputFormatView>
{
    private readonly QrCodeCreatorModel qrCodeCreatorModel;

    public OutputFormatViewModel(QrCodeCreatorModel qrCodeCreatorModel)
    {
        this.qrCodeCreatorModel = qrCodeCreatorModel;
        this.Filename = this.qrCodeCreatorModel.OutputFileName;
        this.UseTimeStamp = this.qrCodeCreatorModel.UseTimeStamp;
        this.OutputFormat = this.qrCodeCreatorModel.OutputFormat;
        this.OutputLocation = this.qrCodeCreatorModel.OutputLocation;
        this.ValidationMessage = string.Empty;
        this.SamplePath = string.Empty;
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        this.OnFilenameChanged(this.Filename);
        this.OnUseTimeStampChanged(this.UseTimeStamp);
        this.OnOutputFormatChanged(this.OutputFormat);
        this.OnOutputLocationChanged(this.OutputLocation);
    }

    [ObservableProperty]
    public partial string Filename { get; set; }

    [ObservableProperty]
    public partial bool UseTimeStamp { get; set; }

    [ObservableProperty]
    public partial OutputFormat OutputFormat { get; set; }

    [ObservableProperty]
    public partial OutputLocation OutputLocation { get; set; }

    [ObservableProperty]
    public partial string ValidationMessage { get; set; }

    [ObservableProperty]
    public partial string SamplePath { get; set; }

    partial void OnFilenameChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            this.ValidationMessage = "The filename cannot be empty.";
            return;
        }

        if (value.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
        {
            this.ValidationMessage = "The filename contains invalid characters.";
            return;
        }

        this.ValidationMessage = string.Empty;
        this.qrCodeCreatorModel.UpdateOutputFilename(this.Filename);
        this.SamplePath = this.qrCodeCreatorModel.OutputFilePath();
    }

    partial void OnUseTimeStampChanged(bool value)
    {
        this.qrCodeCreatorModel.UpdateUseTimeStamp(value);
        this.SamplePath = this.qrCodeCreatorModel.OutputFilePath();
    }

    partial void OnOutputFormatChanged(OutputFormat value)
    {
        this.qrCodeCreatorModel.UpdateOutputFormat(value);
        this.SamplePath = this.qrCodeCreatorModel.OutputFilePath();
    }

    partial void OnOutputLocationChanged(OutputLocation value)
    {
        this.qrCodeCreatorModel.UpdateOutputLocation(value);
        this.SamplePath = this.qrCodeCreatorModel.OutputFilePath();
    }
}
