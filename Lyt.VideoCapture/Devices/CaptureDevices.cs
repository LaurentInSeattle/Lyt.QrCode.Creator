namespace Lyt.VideoCapture.Devices;

public class CaptureDevices
{
    protected readonly BufferPool DefaultBufferPool;

    public CaptureDevices() : this(new DefaultBufferPool()) { }
    
    public CaptureDevices(BufferPool defaultBufferPool) => this.DefaultBufferPool = defaultBufferPool;

    protected virtual List<CaptureDeviceDescriptor> OnEnumerateDescriptors() =>
        Platform.Current switch
        {
            Platforms.Windows => [],
                //new DirectShowDevices(this.DefaultBufferPool).OnEnumerateDescriptors().
                //Concat(new VideoForWindowsDevices(this.DefaultBufferPool).OnEnumerateDescriptors()),
            //Platforms.Linux =>
            //    new V4L2Devices().OnEnumerateDescriptors(),
            //Platforms.MacOS =>
            //    new AVFoundationDevices().OnEnumerateDescriptors(),
           _ => [],
        };

    internal IEnumerable<CaptureDeviceDescriptor> InternalEnumerateDescriptors() =>
        this.OnEnumerateDescriptors();
}
