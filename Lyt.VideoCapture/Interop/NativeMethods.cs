namespace Lyt.VideoCapture.Interop ;

[SuppressUnmanagedCodeSecurity]
internal static class NativeMethods
{
    // https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/aa366535(v=vs.85)

    [DllImport("ntdll")]
    private static extern void RtlCopyMemory(IntPtr dest, IntPtr src, IntPtr length);

    [DllImport("kernel32")]
    private static extern void RtlMoveMemory(IntPtr dest, IntPtr src, IntPtr length);

    [DllImport("libc")]
    private static extern void memcpy(IntPtr dest, IntPtr src, IntPtr length);

    public delegate void CopyMemoryDelegate(IntPtr pDestination, IntPtr pSource, IntPtr length);

    public static unsafe readonly CopyMemoryDelegate CopyMemory =
        Platform.Current == Platforms.Windows ?
            (IntPtr.Size == 4 ? RtlMoveMemory : RtlCopyMemory) :
            memcpy;

    ////////////////////////////////////////////////////////////////////////

    [DllImport("ole32")]
    private static extern IntPtr CoTaskMemAlloc(IntPtr size);

    [DllImport("ole32")]
    private static extern void CoTaskMemFree(IntPtr ptr);

    [DllImport("kernel32")]
    private static extern void RtlZeroMemory(IntPtr ptr, IntPtr size);

    [DllImport("libc")]
    private static extern IntPtr malloc(IntPtr size);

    [DllImport("libc")]
    private static extern void free(IntPtr ptr);

    [DllImport("libc")]
    private static extern IntPtr memset(IntPtr ptr, int c, IntPtr size);

    public delegate IntPtr AllocateMemoryDelegate(IntPtr size);

    public delegate void FreeMemoryDelegate(IntPtr ptr);

    public static readonly AllocateMemoryDelegate AllocateMemory =
        Platform.Current == Platforms.Windows ? AllocateWindows : AllocatePosix;

    public static readonly FreeMemoryDelegate FreeMemory = 
        Platform.Current == Platforms.Windows ? CoTaskMemFree : free;

    private static IntPtr AllocateWindows(IntPtr size)
    {
        var ptr = CoTaskMemAlloc(size);
        RtlZeroMemory(ptr, size);
        return ptr;
    }

    private static IntPtr AllocatePosix(IntPtr size)
    {
        var ptr = malloc(size);
        memset(ptr, 0, size);
        return ptr;
    }

    ////////////////////////////////////////////////////////////////////////

    [DllImport("ole32.dll")]
    public static extern int CoInitialize(IntPtr pvReserved);

    [Flags]
    public enum COINIT
    {
        MULTITHREADED = 0,
        APARTMENTTHREADED = 2,
        DISABLE_OLE1DDE = 4,
        SPEED_OVER_MEMORY = 8,
    }

    [DllImport("ole32", SetLastError=true)]
    public static extern int CoInitializeEx(
        IntPtr pvReserved, COINIT dwCoInit);

    [DllImport("ole32", SetLastError=true)]
    public static extern void CoUninitialize();

    ////////////////////////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct RGBQUAD
    {
        public byte rgbBlue;
        public byte rgbGreen;
        public byte rgbRed;
        public byte rgbReserved;
    }

    private static int GetClrBits(
        short biPlanes, short biBitCount)
    {
        var clrBits = biPlanes * biBitCount;
        if (clrBits != 1)
        {
            if (clrBits <= 4)
            {
                clrBits = 4;
            }
            else if (clrBits <= 8)
            {
                clrBits = 8;
            }
            else if (clrBits <= 16)
            {
                clrBits = 16;
            }
            else if (clrBits <= 24)
            {
                clrBits = 24;
            }
            else
            {
                clrBits = 32;
            }
        }
        return clrBits;
    }


    private static int CalculateClrUsed(
        Compression compression, short biPlanes, short biBitCount)
    {
        if (compression != Compression.BI_RGB)
        {
            return 0;
        }
        else
        {
            var clrBits = GetClrBits(biPlanes, biBitCount);
            return (clrBits < 24) ? (1 << clrBits) : 0;
        }
    }

    [SuppressUnmanagedCodeSecurity]
    private static unsafe int CalculateRawSize(
        Compression compression, short biPlanes, short biBitCount) =>
        sizeof(BITMAPINFOHEADER) +
        CalculateClrUsed(compression, biPlanes, biBitCount) * sizeof(RGBQUAD);

    private static int CalculateImageSize(
        Compression compression,
        int biWidth, int biHeight, short biPlanes, short biBitCount) =>
        compression switch
        {
            Compression.BI_JPEG => 0,
            Compression.BI_PNG => 0,
            Compression.MJPG => 0,
            _ => ((biWidth * GetClrBits(biPlanes, biBitCount) + 31) & ~31) / 8 * biHeight,
        };

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public Compression biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;

        public int GetClrBits() =>
            NativeMethods.GetClrBits(this.biPlanes, this.biBitCount);
        public int CalculateClrUsed() =>
            NativeMethods.CalculateClrUsed(this.biCompression, this.biPlanes, this.biBitCount);
        public int CalculateRawSize() =>
            NativeMethods.CalculateRawSize(this.biCompression, this.biPlanes, this.biBitCount);
        public int CalculateImageSize() =>
            NativeMethods.CalculateImageSize(
                this.biCompression, this.biWidth, this.biHeight,
                this.biPlanes, this.biBitCount);
    }

    [StructLayout(LayoutKind.Sequential, Pack=2)]
    public struct BITMAPFILEHEADER
    {
        public byte bfType0;
        public byte bfType1;
        public int bfSize;
        public short bfReserved1;
        public short bfReserved2;
        public int bfOffBits;
    }

    ////////////////////////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VIDEOINFOHEADER
    {
        public RECT rcSource;
        public RECT rcTarget;
        public int dwBitRate;
        public int dwBitErrorRate;
        public long AvgTimePerFrame;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VIDEOINFOHEADER2
    {
        public RECT rcSource;
        public RECT rcTarget;
        public int dwBitRate;
        public int dwBitErrorRate;
        public long AvgTimePerFrame;
        public int dwInterlaceFlags;
        public int dwCopyProtectFlags;
        public int dwPictAspectRatioX;
        public int dwPictAspectRatioY;
        public int dwControlFlags;    // dwReserved1
        public int dwReserved2;
    }
            
    ////////////////////////////////////////////////////////////////////////



    public static string GetFourCCString(int fourcc)
    {
        if (fourcc < 0x10000000)
        {
            return ((Compression)fourcc).ToString();
        }

        var sb = new StringBuilder();
        sb.Append((char)(byte)fourcc);
        sb.Append((char)(byte)(fourcc >> 8));
        sb.Append((char)(byte)(fourcc >> 16));
        sb.Append((char)(byte)(fourcc >> 24));
        return sb.ToString();
    }
    
    public static VideoCharacteristics? CreateVideoCharacteristics(
        Compression compression,
        int width, int height, int clrBits,
        Fraction framesPerSecond,
        bool isDiscrete = true,
        string? rawPixelFormat = null)
    {
        static PixelFormats GetRGBPixelFormat(int clrBits) =>
            clrBits switch
            {
                8 => PixelFormats.RGB8,
                // BI_RGB is 15bit (RGB555, NOT RGB565)
                // https://docs.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapinfoheader
                16 => PixelFormats.RGB15,
                24 => PixelFormats.RGB24,
                32 => PixelFormats.ARGB32,
                _ => PixelFormats.Unknown,
            };

        var pixelFormat = compression switch
        {
            Compression.BI_RGB => GetRGBPixelFormat(clrBits),
            Compression.RGB2 => GetRGBPixelFormat(clrBits),
            Compression.ARGB => PixelFormats.ARGB32,
            Compression.D3D_RGB24 => PixelFormats.RGB24,
            Compression.D3D_RGB32 => PixelFormats.RGB32,
            Compression.D3D_ARGB32 => PixelFormats.ARGB32,
            Compression.D3D_RGB565 => PixelFormats.RGB16,
            Compression.D3D_RGB555 => PixelFormats.RGB15,
            Compression.MJPG => PixelFormats.JPEG,
            Compression.BI_JPEG => PixelFormats.JPEG,
            Compression.BI_PNG => PixelFormats.PNG,
            Compression.UYVY => PixelFormats.UYVY,
            Compression.YUYV => PixelFormats.YUYV,
            Compression.YUY2 => PixelFormats.YUYV,
            Compression.HDYC => PixelFormats.YUYV,
            Compression.NV12 => PixelFormats.NV12,
            _ => PixelFormats.Unknown,
        };

        return new VideoCharacteristics(
            pixelFormat, width, height,
            framesPerSecond,
            compression.ToString(),
            isDiscrete,
            rawPixelFormat ?? GetFourCCString((int)compression));
    }
    
    public static unsafe VideoCharacteristics? CreateVideoCharacteristics(
        IntPtr pih, Fraction framesPerSecond,
        bool isDiscrete = true,
        string? rawPixelFormat = null)
    {
        var pBih = (BITMAPINFOHEADER*)pih.ToPointer();
        return CreateVideoCharacteristics(
            pBih->biCompression, pBih->biWidth, pBih->biHeight,
            pBih->GetClrBits(), framesPerSecond,
            isDiscrete, rawPixelFormat);
    }

    ////////////////////////////////////////////////////////////////////////

    public static bool GetCompressionAndBitCount(
        PixelFormats format,
        out Compression compression, out short bitCount)
    {
        switch (format)
        {
            case PixelFormats.RGB8:
                compression = Compression.BI_RGB;
                bitCount = 8;
                return true;
            case PixelFormats.RGB15:
                compression = Compression.BI_RGB;
                // BI_RGB & 16bit == RGB555 (Couldn't set RGB565 in DIB)
                // https://docs.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapinfoheader
                bitCount = 16;
                return true;
            case PixelFormats.RGB24:
                compression = Compression.BI_RGB;
                bitCount = 24;
                return true;
            case PixelFormats.RGB32:
                compression = Compression.BI_RGB;
                bitCount = 32;
                return true;
            case PixelFormats.ARGB32:
                compression = Compression.ARGB;
                bitCount = 32;
                return true;
            case PixelFormats.RGB16:
                compression = Compression.D3D_RGB565;
                bitCount = 16;
                return true;
            case PixelFormats.JPEG:
                compression = Compression.MJPG;  // maybe
                bitCount = 24;  // HACK: Specific not found. My web camera is needed.
                return true;
            case PixelFormats.PNG:
                compression = Compression.BI_PNG;
                bitCount = 24;  // ??
                return true;
            case PixelFormats.UYVY:
                compression = Compression.UYVY;
                bitCount = 16;
                return true;
            case PixelFormats.YUYV:
                compression = Compression.YUYV;
                bitCount = 16;
                return true;
            case PixelFormats.NV12:
                compression = Compression.NV12;
                bitCount = 12;
                return true;
            default:
                compression = default;
                bitCount = 0;
                return false;
        }
    }
}
