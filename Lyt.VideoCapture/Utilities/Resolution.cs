namespace Lyt.VideoCapture.Utilities;

// https://en.wikipedia.org/wiki/Computer_display_standard
//
public readonly struct Resolution : IEquatable<Resolution>, IComparable<Resolution>
{
    public readonly int Width;
    public readonly int Height;

    private Resolution(int width, int height)
    {
        this.Width = width;
        this.Height = height;
    }

    public static Resolution Create(int width, int height) => new (width, height);

    public override int GetHashCode() => this.Width.GetHashCode() ^ this.Height.GetHashCode();

    public bool Equals(Resolution other) =>
        this.Width.Equals(other.Width) &&
        this.Height.Equals(other.Height);

    public override bool Equals(object? obj) => obj is Resolution other && this.Equals(other);

    public int CompareTo(Resolution other) => (this.Width * this.Height).CompareTo(other.Width * other.Height);

    public override string ToString() => $"{this.Width},{this.Height}";
    
    public static readonly HashSet<Resolution> DefactoStandardResolutions = new()
    {
        Resolution.Create(10240, 4320),
        Resolution.Create(7680, 4800),
        Resolution.Create(7680, 4320),   // 8K
        Resolution.Create(7680, 3200),
        Resolution.Create(6400, 4800),
        Resolution.Create(6400, 4096),
        Resolution.Create(5120, 4096),
        Resolution.Create(5120, 3200),
        Resolution.Create(5120, 2880),   // 5K
        Resolution.Create(5120, 2160),
        Resolution.Create(4500, 3000),
        Resolution.Create(4096, 3072),
        Resolution.Create(4096, 2160),   // 4K
        Resolution.Create(4000, 3000),   // 12MP
        Resolution.Create(3840, 2400),
        Resolution.Create(3840, 2160),   // 8MP
        Resolution.Create(3840, 1600),
        Resolution.Create(3440, 1440),
        Resolution.Create(3200, 2400),
        Resolution.Create(3200, 2048),
        Resolution.Create(3072, 2048),
        Resolution.Create(3000, 2000),
        Resolution.Create(2960, 1440),
        Resolution.Create(2880, 1800),
        Resolution.Create(2880, 1440),
        Resolution.Create(2688, 1520),  // 4MP
        Resolution.Create(2592, 1944),  // 5MP
        Resolution.Create(2560, 2048),
        Resolution.Create(2560, 1600),
        Resolution.Create(2560, 1440),
        Resolution.Create(2560, 1080),
        Resolution.Create(2160, 1440),
        Resolution.Create(2048, 1536),  // 3MP
        Resolution.Create(2048, 1152),
        Resolution.Create(2048, 1080),  // 2K
        Resolution.Create(1920, 1440),
        Resolution.Create(1920, 1280),
        Resolution.Create(1920, 1200),
        Resolution.Create(1920, 1080),  // 1080p
        Resolution.Create(1680, 1050),
        Resolution.Create(1600, 1200),  // 2MP
        Resolution.Create(1600, 900),
        Resolution.Create(1440, 900),
        Resolution.Create(1400, 1050),
        Resolution.Create(1366, 768),
        Resolution.Create(1360, 768),
        Resolution.Create(1280, 1024),  // 1.3MP
        Resolution.Create(1280, 960),   // 960p
        Resolution.Create(1280, 720),   // 720p
        Resolution.Create(1152, 900),
        Resolution.Create(1152, 870),
        Resolution.Create(1152, 864),
        Resolution.Create(1056, 400),
        Resolution.Create(1024, 768),
        Resolution.Create(960, 480),   // 960H
        Resolution.Create(832, 624),
        Resolution.Create(800, 600),
        Resolution.Create(720, 576),
        Resolution.Create(720, 480),    // D1
        Resolution.Create(720, 400),
        Resolution.Create(720, 350),
        Resolution.Create(640, 480),    // VGA
        Resolution.Create(640, 400),
        Resolution.Create(640, 350),
        Resolution.Create(640, 200),
        Resolution.Create(512, 384),
        Resolution.Create(480, 272),
        Resolution.Create(480, 720),
        Resolution.Create(320, 240),    // QVGA
        Resolution.Create(320, 200),
        Resolution.Create(240, 160),
        Resolution.Create(160, 200),
        Resolution.Create(160, 144),
        Resolution.Create(160, 128),
        Resolution.Create(160, 120),
    };

    }


