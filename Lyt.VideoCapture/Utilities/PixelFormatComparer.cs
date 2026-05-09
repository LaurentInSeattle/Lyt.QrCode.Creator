namespace Lyt.VideoCapture.Utilities;

public sealed class PixelFormatComparer : IComparer<PixelFormats>
{
    private static int GetComparableCode (PixelFormats pixelFormat) =>
        pixelFormat switch
        {
            PixelFormats.RGB8 => 0,
            PixelFormats.RGB16 => 10,
            PixelFormats.JPEG => 20,
            PixelFormats.RGB24 => 40,
            PixelFormats.RGB32 => 50,
            PixelFormats.ARGB32 => 60,
            PixelFormats.PNG => 70,
            _ => 30,
        };

    public int Compare(PixelFormats x, PixelFormats y) =>
        PixelFormatComparer.GetComparableCode(x).CompareTo(GetComparableCode(y));

    public static readonly PixelFormatComparer Instance = new ();
}
