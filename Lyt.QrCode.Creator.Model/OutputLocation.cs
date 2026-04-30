namespace Lyt.QrCode.Creator.Model;

public enum OutputLocation
{
    Desktop,
    Documents,
    Downloads,
}

public static class OutputLocationExtensions
{
    public static string FolderPath(this OutputLocation location)
        => location switch
        {
            OutputLocation.Desktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            OutputLocation.Documents => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            OutputLocation.Downloads => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads",
            _ => throw new ArgumentOutOfRangeException(nameof(location), $"Unexpected output location value: {location}"),
        };
}