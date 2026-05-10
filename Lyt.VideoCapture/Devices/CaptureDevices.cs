namespace Lyt.VideoCapture.Devices;

public class CaptureDevices(BufferPool defaultBufferPool)
{
    protected readonly BufferPool DefaultBufferPool = defaultBufferPool;

    public CaptureDevices() : this(new DefaultBufferPool()) { }

    public List<CaptureDeviceDescriptor> EnumerateDescriptors() => 
        Platform.Current switch
        {
            Platforms.Windows => DirectShowDevices.EnumerateDescriptors(this.DefaultBufferPool),
            //Concat(new VideoForWindowsDevices(this.DefaultBufferPool).OnEnumerateDescriptors()),
            //Platforms.Linux =>
            //    new V4L2Devices().OnEnumerateDescriptors(),
            //Platforms.MacOS =>
            //    new AVFoundationDevices().OnEnumerateDescriptors(),
            _ => [],
        };
}
