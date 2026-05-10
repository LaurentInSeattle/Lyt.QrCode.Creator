namespace Lyt.VideoCapture.Console.Test;

using static System.Console;

public static class Program
{
	private const string DefaultFileName = "C:\\Users\\Laurent\\Desktop\\QrTests\\capture";

	public static async Task<int> Main(string[] args)
	{
		WriteLine("Test Video Capture");

		try
		{
			return await TestRun(DefaultFileName, default);
		}
		catch (Exception ex)
		{
			WriteLine(ex.ToString());
			return Marshal.GetHRForException(ex);
		}
	}

	private static async Task<int> TestRun(string fileName, CancellationToken ct)
	{
		// Initialize and detection capture devices.

		// Step 1: Enumerate capture devices and filter by device type
		var devices = new CaptureDevices();
		var descriptors = 
			devices.EnumerateDescriptors()
			// Only DirectShow device.
			.Where(d => d.DeviceType == DeviceTypes.DirectShow)
			.ToList();

		// pickup first device 
		var selectedDevice = descriptors.FirstOrDefault();
		if (selectedDevice == null)
		{
			WriteLine($"Could not detect any capture interfaces.");
			return 0;
		}

		// Filter characteristics by pixel format.
		var characteristics =
			selectedDevice.Characteristics
			// Only UYVY characteristics.
			.Where(c => c.PixelFormat == PixelFormats.YUYV)
			//// Only MJPEG characteristics.
			//.Where(c => c.PixelFormat == PixelFormats.JPEG)
			.ToList();
		if (characteristics.Count == 0)
		{
			WriteLine($"Could not select color format characteristics.");
			return 0;
		}

		// Select best characteristics, first by size
		var sorted =
			(from c in characteristics 
			 orderby c.Width * c.Height descending
			 orderby (double) c.FramesPerSecond descending
			 select c).ToList();
		var bestCharacteristics = sorted[0];

		WriteLine($"Selected capture device: {selectedDevice}, {bestCharacteristics}");

		// Open device 
		var tcs = new TaskCompletionSource<byte[]>();
		using var captureDevice = await selectedDevice.OpenAsync(
			bestCharacteristics,
			bufferScope =>
			{
				// Pixel buffer has arrived.

				// Step 3-2: Copy image data binary:
				var image = bufferScope.Buffer.CopyImage();
				WriteLine($"Captured {image.Length} bytes.");

				// Step 3-3: Relay to outside continuation.
				tcs.TrySetResult(image);

				// If you output to each files from continuous image data,
				// it would be easier to output directly to file here.
				// In that case, use:
				// * `isScattering` argument to true.
				// * `maxQueuingFrames` argument.
				// * `bufferScope.ReleaseNow()` method.
				// and be careful not to cause frame dropping.
			},
			ct);

		WriteLine($"Device opened.");

		// Step 4: Start capturing:
		await captureDevice.StartAsync(ct);

		WriteLine($"Device started.");

		// Step 5: Waiting to continue:
		var image = await tcs.Task;

		// Step 6: Stop capturing:
		await captureDevice.StopAsync(ct);

		WriteLine($"Device stopped.");


//#if false
//        // Step 2-1: Request video characteristics strictly:
//        // Will raise exception when parameters are not accepted.
//        var characteristics = new VideoCharacteristics(
//            PixelFormats.JPEG, 1920, 1080, 60);
//#else
//        // Step 2-2: Or, you could choice from device descriptor:
//        var characteristics0 = descriptor0.Characteristics.
//            //Where(c => c.PixelFormat == PixelFormats.JPEG).  // Only MJPEG characteristics.
//            FirstOrDefault(c => c.PixelFormat != PixelFormats.Unknown);
//        if (characteristics0 == null)
//        {
//            WriteLine($"Could not select primary characteristics.");
//            return 0;
//        }
//#endif

//        WriteLine($"Selected capture device: {descriptor0}, {characteristics0}");

//        ///////////////////////////////////////////////////////////////
//        // Start capture and get one image.

//#if true
//        // Step 3: New interface: Simple take one shot.
//        var image = await descriptor0.TakeOneShotAsync(characteristics0, ct);

//        WriteLine($"Captured {image.Length} bytes.");
//#else
//        // Equivalent implementation

//        // Step 3-1: Open the capture device with specific characteristics:
//        var tcs = new TaskCompletionSource<byte[]>();
//        using var captureDevice = await descriptor0.OpenAsync(
//            characteristics0,
//            bufferScope =>
//            {
//                ////////////////////////////////////////////////
//                // Pixel buffer has arrived.

//                // Step 3-2: Copy image data binary:
//                var image = bufferScope.Buffer.CopyImage();

//                Console.WriteLine($"Captured {image.Length} bytes.");

//                // Step 3-3: Relay to outside continuation.
//                tcs.TrySetResult(image);

//                // If you output to each files from continuous image data,
//                // it would be easier to output directly to file here.
//                // In that case, use:
//                // * `isScattering` argument to true.
//                // * `maxQueuingFrames` argument.
//                // * `bufferScope.ReleaseNow()` method.
//                // and be careful not to cause frame dropping.
//            },
//            ct);

//        // Step 4: Start capturing:
//        await captureDevice.StartAsync(ct);

//        Console.WriteLine($"Device opened.");

//        // Step 5: Waiting to continue:
//        var image = await tcs.Task;

//        // Step 6: Stop capturing:
//        await captureDevice.StopAsync(ct);

//        Console.WriteLine($"Device stopped.");
//#endif

//        ///////////////////////////////////////////////////////////////
//        // Save image data to file.

//        // Step 7: Construct storing file name:
//        string extension = characteristics0.PixelFormat switch
//        {
//            PixelFormats.JPEG => ".jpg",
//            PixelFormats.PNG => ".png",   // (Very rare device, I dont know)
//            _ => ".bmp",
//        };
//        string path = $"{fileName}{extension}";

//        // Step 8: Write to the file:
//        using var fs = new FileStream(
//            path,
//            FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite,
//            65536, true);
//        await fs.WriteAsync(image, 0, image.Length, ct);
//        await fs.FlushAsync(ct);

//        WriteLine($"The image written to: {path}.");

		return 0;
	}

}

/*
 * 
 * 
https://github.com/Coloryr/FFClient/blob/master/FFClient.Avalonia/FFClient.Avalonia/Views/FFClientControl.axaml.cs


 * 
# define Y_OFFSET   16
# define UV_OFFSET 128
# define YUV2RGB_11  298
# define YUV2RGB_12   -1
# define YUV2RGB_13  409
# define YUV2RGB_22 -100
# define YUV2RGB_23 -210
# define YUV2RGB_32  519
# define YUV2RGB_33    0


	while(pixelnum--) {
	  int y, u, v;
	  int uv_r, uv_g, uv_b;
	  u=yuvdata[chU]-UV_OFFSET;
	  v=yuvdata[chV]-UV_OFFSET;
	  uv_r=YUV2RGB_12*u+YUV2RGB_13*v;
	  uv_g=YUV2RGB_22*u+YUV2RGB_23*v;
	  uv_b=YUV2RGB_32*u+YUV2RGB_33*v;

	  // 1st pixel
	  y=YUV2RGB_11*(yuvdata[chY0] -Y_OFFSET);
	  pixels[chR] = CLAMP((y + uv_r) >> 8); // r
	  pixels[chG] = CLAMP((y + uv_g) >> 8); // g
	  pixels[chB] = CLAMP((y + uv_b) >> 8); // b
	  pixels+=3;
	  // 2nd pixel
	  y=YUV2RGB_11*(yuvdata[chY1] -Y_OFFSET);
	  pixels[chR] = CLAMP((y + uv_r) >> 8); // r
	  pixels[chG] = CLAMP((y + uv_g) >> 8); // g
	  pixels[chB] = CLAMP((y + uv_b) >> 8); // b
	  pixels+=3;

	  yuvdata+=4;
	}


//Translate yuyv format to rgb888 format
static void yuyv_to_rgb_pixel(unsigned char *yuyv, unsigned char *rgb);

bool yuyv_to_rgb(unsigned char *yuyv, unsigned char *rgb, int height, int width)
{
	unsigned char temp = 0;
	long yuv_size = height * width * 2;
	long rgb_size = height * width * 3;

	if (yuyv == NULL || rgb == NULL)
	return false;

	for (int i = 0, j = 0; i < rgb_size && j < yuv_size; i += 6, j += 4)
	{
	yuyv_to_rgb_pixel(&yuyv[j], &rgb[i]);
	}
	return true;
}

bool yuyv_to_bgr(unsigned char *yuyv, unsigned char *rgb, int height, int width)
{
	unsigned char temp = 0;
	long yuv_size = height * width * 2;
	long rgb_size = height * width * 3;

	if (yuyv == NULL || rgb == NULL)
	return false;
	for (int i = 0, j = 0; i < rgb_size && j < yuv_size; i += 6, j += 4)
	{
	yuyv_to_rgb_pixel(&yuyv[j], &rgb[i]);
	temp = rgb[i + 0];
	rgb[i + 0] = rgb[i + 2];
	rgb[i + 2] = temp;
	temp = rgb[i + 3];
	rgb[i + 3] = rgb[i + 5];
	rgb[i + 5] = temp;
	}
	return true;
}

void yuyv_to_rgb_pixel(unsigned char *yuyv, unsigned char *rgb)
{
	int y, v, u;
	float r, g, b;

	y = yuyv[0]; //y0
	u = yuyv[1]; //u0
	v = yuyv[3]; //v0

	r = y + 1.4065 * (v - 128);			     //r0
	g = y - 0.3455 * (u - 128) - 0.7169 * (v - 128); //g0
	b = y + 1.1790 * (u - 128);			     //b0

	if (r < 0)
	r = 0;
	else if (r > 255)
	r = 255;
	if (g < 0)
	g = 0;
	else if (g > 255)
	g = 255;
	if (b < 0)
	b = 0;
	else if (b > 255)
	b = 255;

	rgb[0] = (unsigned char)r;
	rgb[1] = (unsigned char)g;
	rgb[2] = (unsigned char)b;

	//second pixel
	u = yuyv[1]; //u0
	y = yuyv[2]; //y1
	v = yuyv[3]; //v0

	r = y + 1.4065 * (v - 128);			     //r1
	g = y - 0.3455 * (u - 128) - 0.7169 * (v - 128); //g1
	b = y + 1.1790 * (u - 128);			     //b1

	if (r < 0)
	r = 0;
	else if (r > 255)
	r = 255;
	if (g < 0)
	g = 0;
	else if (g > 255)
	g = 255;
	if (b < 0)
	b = 0;
	else if (b > 255)
	b = 255;

	rgb[3] = (unsigned char)r;
	rgb[4] = (unsigned char)g;
	rgb[5] = (unsigned char)b;
}
 */