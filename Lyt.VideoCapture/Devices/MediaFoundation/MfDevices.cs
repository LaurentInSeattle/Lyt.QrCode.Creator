namespace Lyt.VideoCapture.Devices.MediaFoundation;

using global::MediaFoundation;
using global::MediaFoundation.Misc;
using global::MediaFoundation.ReadWrite;
using global::MediaFoundation.Alt;
using global::MediaFoundation.Transform;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CS8604 // Possible null reference argument.

public static class MediaFoundationDevices
{
    internal static List<CaptureDeviceDescriptor> EnumerateDescriptors(BufferPool bufferPool) 
    {
        var deviceDescriptors = new List<CaptureDeviceDescriptor>();
        var devices = new List<MfDevice>();

        try
        {
            var hr = (HResult) NativeMethods.CoInitializeEx((IntPtr)null, NativeMethods.COINIT.SPEED_OVER_MEMORY);            
            MF.Startup();

            IMFAttributes attributes = MF.CreateAttributes(1);
            Guid MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID = new Guid("8ac3587a-4ae7-42d8-99e0-0a6013eef90f");
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
        }
        catch (Exception ex)
        { 
        }

        return deviceDescriptors;
    }
}

#pragma warning restore CA1416 
#pragma warning restore CA8604
#pragma warning restore CA8625 
