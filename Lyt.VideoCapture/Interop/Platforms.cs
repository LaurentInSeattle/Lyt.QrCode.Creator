namespace Lyt.VideoCapture.Interop;

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
        if (OperatingSystem.IsWindows())
        {
            return Platforms.Windows;
        }
        else if (OperatingSystem.IsMacOS())
        {
            return Platforms.MacOS;
        }
        else if (OperatingSystem.IsLinux())
        {
            return Platforms.Linux;
        }

        // else if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS() || OperatingSystem.IsBrowser())
        // or Unknown platform
        return Platforms.Other;
    }
}