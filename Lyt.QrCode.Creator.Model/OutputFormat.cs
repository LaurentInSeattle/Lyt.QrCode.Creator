namespace Lyt.QrCode.Creator.Model;

public enum OutputFormat
{
    Png = 0,
    Jpeg = 1,
    Bmp = 2,

    // Pdf = 3,
}

public static class OutputFormatExtensions
{
    public static string FileExtension(this OutputFormat format)
        => format switch
        {
            OutputFormat.Png => "png",
            OutputFormat.Jpeg => "jpg",
            OutputFormat.Bmp => "bmp",
            // OutputFormat.Pdf => "pdf",
            _ => throw new ArgumentOutOfRangeException(nameof(format), $"Not expected output format value: {format}"),
        };
}