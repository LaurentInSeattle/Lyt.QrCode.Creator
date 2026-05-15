namespace Lyt.VideoCapture.Devices;

public class CaptureDevices
{
    protected readonly BufferPool bufferPool = new();

    public List<CaptureDeviceDescriptor> Enumerate()
    {
        // Concat(new VideoForWindowsDevices(this.DefaultBufferPool).OnEnumerateDescriptors()),
        //Platforms.Linux =>
        //    new V4L2Devices().OnEnumerateDescriptors(),
        //Platforms.MacOS =>
        //    new AVFoundationDevices().OnEnumerateDescriptors(),

        List<CaptureDeviceDescriptor>list = [];
        switch (Platform.Current)
        {   
            case Platforms.Windows:
                list.AddRange(DirectShowDevices.Enumerate(this.bufferPool));
                list.AddRange(MediaFoundationDevices.Enumerate(this.bufferPool)); 
                return list;

            case Platforms.Linux:
                break;
            case Platforms.MacOS:
                break;
            case Platforms.Other:
                break;
            default:
                break;
        }

        return list;
    }
}
