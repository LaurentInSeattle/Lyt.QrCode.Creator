namespace Lyt.VideoCapture.Devices.MediaFoundation;

using global::MediaFoundation;
using global::MediaFoundation.ReadWrite;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CA1416 // Validate platform compatibility

public static class MediaFoundationDevices
{
    internal static List<CaptureDeviceDescriptor> Enumerate(BufferPool bufferPool) 
    {
        var deviceDescriptors = new List<CaptureDeviceDescriptor>();
        var devices = new List<MfDevice>();

        try
        {
            var hr = (HResult) NativeMethods.CoInitialize((IntPtr)null);            
            MF.Startup();

            IMFAttributes attributes = MF.CreateAttributes(1);
            Guid MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID = new ("8ac3587a-4ae7-42d8-99e0-0a6013eef90f");
            hr = attributes.SetGUID(
                MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
                MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            hr.ThrowExceptionOnError();
            hr = MF.EnumDeviceSources(attributes, out IMFActivate[] sourceActivate);
            hr.ThrowExceptionOnError();
            Debug.WriteLine("Device(s) Found: " + sourceActivate.Length.ToString());
            int deviceIndex = 0;
            foreach (var source in sourceActivate)
            {
                hr = source.GetAllocatedString(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME, out string friendlyName);
                hr.ThrowExceptionOnError();
                hr = source.GetAllocatedString(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_SYMBOLIC_LINK, out string symbolicName);
            
                var device = new MfDevice(symbolicName, friendlyName)
                { 
                    FriendlyName = friendlyName,
                    SymbolicName = symbolicName,
                    Index = deviceIndex
                };

                devices.Add(device);
                Debug.WriteLine(string.Format("Found: {0} ({1})", friendlyName, symbolicName));

                Guid riidMediaSource = Helper.GetGuid<IMFMediaSource>();
                hr = source.ActivateObject(riidMediaSource, out object ppv);
                hr.ThrowExceptionOnError();
                if (ppv is not IMFMediaSource mediaSource)
                {
                    throw new InvalidOperationException("Failed to activate media source.");
                }

                hr = MF.CreateSourceReaderFromMediaSource(mediaSource, null, out IMFSourceReader sourceReader);
                hr.ThrowExceptionOnError();
                hr = device.EnumerateModes(sourceReader);
                hr.ThrowExceptionOnError();

                ++deviceIndex;

                // Clean up 
                Marshal.ReleaseComObject(mediaSource);
                Marshal.ReleaseComObject(sourceReader);
            }

            foreach (var source in sourceActivate)
            {
                Marshal.ReleaseComObject(source);
            }

            // DO NOT do this:  

            //     Marshal.ReleaseComObject(source);
            // The device hold a ref to the source that will be released when the C# device is disposed

            // No release of sourceActivate (Don't try!) 
            Marshal.ReleaseComObject(attributes);

            // Using the list of devices, create the descriptors
            foreach (var device in devices)
            {
                // for each device mode, create VideoCharacteristics 
                List<VideoCharacteristics> videoCharacteristics = []; 
                foreach (var mode in device.SupportedModes)
                {
                    Debug.WriteLine(string.Format("  Mode: {0}x{1} @ {2}fps", mode.Width, mode.Height, mode.FrameRate));
                    videoCharacteristics.Add(mode.ToVideoCharacteristics());
                }

                // create a descriptor and add it to the list of descriptors
                var descriptor = new MfDeviceDescriptor(
                    device,
                    device.SymbolicName,
                    device.FriendlyName,
                    device.FriendlyName,
                    [.. videoCharacteristics],
                    bufferPool);
                deviceDescriptors.Add(descriptor);
            }
        }
        catch (Exception ex)
        { 
            Debug.WriteLine("Error enumerating Media Foundation devices: " + ex.ToString());
        }

        return deviceDescriptors;
    }
}

#pragma warning restore CA8625 
#pragma warning restore CA1416 
