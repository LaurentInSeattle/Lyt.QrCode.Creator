namespace Lyt.VideoCapture.Devices.MediaFoundation;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CS8604 // Possible null reference argument.

public sealed class MfDeviceDescriptor : CaptureDeviceDescriptor
{
    private readonly string devicePath;

    internal MfDeviceDescriptor(
        string devicePath, string name, string description,
        VideoCharacteristics[] characteristics,
        BufferPool defaultBufferPool) :
        base(name, description, characteristics, defaultBufferPool) =>
        this.devicePath = devicePath;

    public override object Identity => this.devicePath;

    public override DeviceTypes DeviceType => DeviceTypes.MediaFoundation;

    protected override Task<CaptureDevice> OnOpenWithFrameProcessorAsync(
        VideoCharacteristics characteristics, 
        TranscodeFormats transcodeFormat, 
        FrameProcessor frameProcessor, 
        CancellationToken ct)
    {
        return Task.FromResult<CaptureDevice>(new MfDevice(this, ""));
    }
}

#pragma warning restore CA1416 
#pragma warning restore CA8604
#pragma warning restore CA8625 
