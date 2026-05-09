namespace Lyt.VideoCapture.Utilities;

public enum Compression
{
    BI_RGB = 0,          // BI_RGB
    BI_JPEG = 4,         // BI_JPEG
    BI_PNG = 5,          // BI_PNG
    D3D_RGB24 = 0x00000014,  // D3D
    D3D_RGB32 = 0x00000016,  // D3D
    D3D_ARGB32 = 0x00000015, // D3D
    D3D_RGB565 = 0x00000017, // D3D
    D3D_RGB555 = 0x00000018, // D3D
    ARGB = 0x42475241,   // FOURCC
    RGB2 = 0x32424752,   // FOURCC
    YUY2 = 0x32595559,   // FOURCC
    YUYV = 0x56595559,   // FOURCC
    UYVY = 0x59565955,   // FOURCC
    MJPG = 0x47504A4D,   // FOURCC
    HDYC = 0x43594448,   // FOURCC (BlackMagic input (UYVY))
    NV12 = 0x3231564E,   // FOURCC
}

