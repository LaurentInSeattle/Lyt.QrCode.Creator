namespace Lyt.VideoCapture.Devices.MediaFoundation;

//#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
//#pragma warning disable CA1416 // Validate platform compatibility
//#pragma warning disable CS8604 // Possible null reference argument.

public sealed class MfDeviceDescriptor : CaptureDeviceDescriptor
{
    private readonly MfDevice device;
    private readonly string devicePath;

    internal MfDeviceDescriptor(
        MfDevice device,         
        string devicePath, string name, string description,
        VideoCharacteristics[] characteristics, BufferPool bufferPool) :
        base(name, description, characteristics, bufferPool)
    {
        this.device = device;
        this.devicePath = devicePath;
    }

    public override object Identity => this.devicePath;

    public override DeviceTypes DeviceType => DeviceTypes.MediaFoundation;

    protected override Task<CaptureDevice> OnOpenWithFrameProcessorAsync(
        VideoCharacteristics characteristics, 
        TranscodeFormats transcodeFormat, 
        FrameProcessor frameProcessor, 
        CancellationToken ct)
        => this.InternalOnOpenWithFrameProcessorAsync(
            this.device, characteristics, transcodeFormat, frameProcessor, ct);
}

//#pragma warning restore CA1416 
//#pragma warning restore CA8604
//#pragma warning restore CA8625 
