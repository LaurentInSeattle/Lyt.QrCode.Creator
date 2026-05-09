namespace Lyt.VideoCapture.Interop;

// See:  https://stackoverflow.com/questions/38790802/determine-operating-system-in-net-core

public enum Platforms
{
    Windows,
    Linux,
    MacOS,
    Other,
}

public static class Platform
{
    public static readonly Platforms Current = GetRuntimePlatform();

    private static Platforms GetRuntimePlatform()
    {
        var windir = Environment.GetEnvironmentVariable("windir");
        if (!string.IsNullOrEmpty(windir) &&
            windir.Contains(Path.DirectorySeparatorChar.ToString()) &&
            Directory.Exists(windir))
        {
            return Platforms.Windows;
        }
        else if (File.Exists(@"/proc/sys/kernel/ostype"))
        {
            var osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
            if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
            {
                return Platforms.Linux;
            }
            else
            {
                return Platforms.Other;
            }
        }
        else if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
        {
            return Platforms.MacOS;
        }
        else
        {
            return Platforms.Other;
        }
    }
} 