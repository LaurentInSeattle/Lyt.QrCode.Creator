namespace Lyt.VideoCapture.Devices.DirectShow;

using static Lyt.VideoCapture.Interop.DirectShow.NativeMethods_DirectShow;

public static class DirectShowDevices 
{
    internal static List<CaptureDeviceDescriptor> Enumerate(BufferPool bufferPool) =>
        [.. NativeMethods_DirectShow
        .EnumerateDeviceMoniker(CLSID_VideoInputDeviceCategory)
        .Collect(moniker => moniker.GetPropertyBag() is { } pb ?
            pb.SafeReleaseBlock(pb =>
                pb.GetValue("FriendlyName", default(string))?.Trim() is { } n &&
                (string.IsNullOrEmpty(n) ? "Unknown" : n!) is { } name &&
                pb.GetValue("DevicePath", default(string))?.Trim() is { } devicePath ?
                    (CaptureDeviceDescriptor)new DirectShowDeviceDescriptor(
                        devicePath, name,
                        pb.GetValue("Description", default(string))?.Trim() ?? $"{name} (DirectShow)",
                        moniker.BindToObject(
                            null, null, in IID_IBaseFilter, out var cs) == 0 &&
                        cs is IBaseFilter captureSource ?
                            captureSource.SafeReleaseBlock(
                                captureSource => captureSource.EnumeratePins().
                                Collect(pin =>
                                    pin.GetPinInfo() is { } pinInfo &&
                                    pinInfo.dir == PIN_DIRECTION.Output ?
                                        pin : null).
                                SelectMany(pin =>
                                    pin.EnumerateFormats().
                                    Collect(format => format.CreateVideoCharacteristics())).
                                Distinct().
                                OrderByDescending(vc => vc).
                                ToArray()) :
                            [],
                        bufferPool) :
                    null) :
            null)];
}
