namespace Lyt.VideoCapture.Devices;

public class CaptureDevices
{
    protected readonly BufferPool bufferPool = new();

    public List<CaptureDeviceDescriptor> Enumerate() => 
        Platform.Current switch
        {
            Platforms.Windows => 
                // DirectShowDevices.EnumerateDescriptors(this.DefaultBufferPool)
                MediaFoundationDevices.Enumerate(this.bufferPool),
            // Concat(new VideoForWindowsDevices(this.DefaultBufferPool).OnEnumerateDescriptors()),
            //Platforms.Linux =>
            //    new V4L2Devices().OnEnumerateDescriptors(),
            //Platforms.MacOS =>
            //    new AVFoundationDevices().OnEnumerateDescriptors(),
            _ => [],
        };
}
