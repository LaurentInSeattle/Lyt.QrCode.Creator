
using Avalonia;

using Lyt.Video.Capture.MediaFoundation;
using Lyt.Video.Shared.Abstractions;

using System;

namespace Lyt.QrCode.Creator.Desktop;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .With(new SkiaOptions() { MaxGpuResourceSizeBytes = 2L * 1024L * 1024L * 1024L }) // 2 GB 
            .LogToTrace()
            .WithDeveloperTools();

    // Artificially creates a dependency 
    public static ICaptureDeviceExplorer CaptureDeviceExplorer => new MediaFoundationDeviceExplorer();
}
