// Consider moving this into the Avalonia area and create a new library for Avalonia images and media.
namespace Lyt.QrCode.Creator.Utilities;

public static class ImagingUtilities
{
    /// <summary> Saves the specified visual as an image to the given file path. </summary>
    /// <param name="visual">The visual to save as an image.</param>
    /// <param name="filePath">The file path where the image will be saved.</param>
    public static void SaveAsImage(this Visual visual, string filePath)
    {
        int scaling = 4;
        int width = (int)visual.Bounds.Width;
        int height = (int)visual.Bounds.Height;

        // Define the size based on the control's bounds
        // and Scale WAY UP to avoid rendering artefacts 
        var pixelSize = new PixelSize(scaling * width, scaling * height);

        // Create the render target
        double dpi = scaling * 96.0; // 96.0 == Standard DPI for most displays
        using var renderTarget = new RenderTargetBitmap(pixelSize, new Vector(dpi, dpi));

        // Render the visual to the upscaled bitmap
        renderTarget.Render(visual);

        // Rescale back to original size 
        WriteableBitmap writeableBitmap = WriteableFromBitmap(renderTarget);

        // CreateScaledBitmap fails on writeable bitmaps and render targets 
        Bitmap bitmapForScaling = FromWriteableBitmap(writeableBitmap);
        var pixelSizeFinal = new PixelSize(width, height);
        Bitmap scaledBitmap = bitmapForScaling.CreateScaledBitmap(pixelSizeFinal, BitmapInterpolationMode.HighQuality);

        // Save to the specified file path
        scaledBitmap.Save(filePath);
    }

    public static Bitmap DecodeBitmap(IEnumerable<byte> blob)
    {
        using var stream = new MemoryStream([.. blob]);
        return new Bitmap(stream);
    }

    public static Bitmap ThumbnailBitmapFrom(Bitmap bitmap, int width, int height)
    {
        double scale = Math.Min(width / (double)bitmap.Size.Width, height / (double)bitmap.Size.Height);
        int scaledWidth = (int)(bitmap.Size.Width * scale);
        int scaledHeight = (int)(bitmap.Size.Height * scale);
        var resized = bitmap.CreateScaledBitmap(new PixelSize(scaledWidth, scaledHeight), BitmapInterpolationMode.MediumQuality);
        return resized;
    }

    public static Bitmap FromWriteableBitmap(WriteableBitmap writeableBitmap)
    {
        using ILockedFramebuffer fb = writeableBitmap.Lock();
        var format = (PixelFormat)writeableBitmap.Format!;
        nint data = fb.Address;
        var bitmap = new Bitmap(
            format,
            AlphaFormat.Opaque,
            data,
            writeableBitmap.PixelSize,
            writeableBitmap.Dpi,
            fb.RowBytes);
        return bitmap;
    }

    public static WriteableBitmap WriteableFromBitmap(Bitmap bitmap)
    {
        var writeableBitmap = new WriteableBitmap(
            bitmap.PixelSize,
            bitmap.Dpi,
            bitmap.Format
        );

        using (ILockedFramebuffer fb = writeableBitmap.Lock())
        {
            bitmap.CopyPixels(fb);
        }

        return writeableBitmap;
    }

    public static unsafe WriteableBitmap Duplicate(this WriteableBitmap source)
        => source.Crop(new PixelRect(0, 0, source.PixelSize.Width, source.PixelSize.Height));

    public static unsafe WriteableBitmap Crop(this WriteableBitmap source, PixelRect roi)
    {
        try
        {
            var size = source.PixelSize;
            var format = source.Format ?? throw new InvalidOperationException("Source bitmap has no format");
            var alphaFormat = source.AlphaFormat ?? throw new InvalidOperationException("Source bitmap has no alpha format");
            using ILockedFramebuffer fb = source.Lock();

            int stride = fb.RowBytes;
            int minStride = (format.BitsPerPixel * size.Width + 7) / 8;
            if (minStride > stride)
            {
                throw new Exception(nameof(stride));
            }

            byte* srcData = (byte*)fb.Address;
            int bytesPerPixel = format.BitsPerPixel / 8;
            byte[] destBytes = new byte[roi.Width * roi.Height * format.BitsPerPixel / 8];
            fixed (byte* dstData = destBytes)
            {
                int dstRow = 0;
                for (int y = roi.Y; y < roi.Y + roi.Height; ++y)
                {
                    int dstCol = 0;
                    for (int x = roi.X; x < roi.X + roi.Width; ++x)
                    {
                        int dstIndex = dstRow * roi.Width * bytesPerPixel + dstCol * bytesPerPixel;
                        int srcIndex = y * size.Width * bytesPerPixel + x * bytesPerPixel;
                        for (int byteIndex = 0; byteIndex < bytesPerPixel; ++byteIndex)
                        {
                            dstData[dstIndex++] = srcData[srcIndex++];
                        }

                        ++dstCol;
                    }

                    ++dstRow;
                }

                var pixelSize = new PixelSize(roi.Width, roi.Height);
                var bitmap =
                    new WriteableBitmap(
                        format, alphaFormat, (IntPtr)dstData, pixelSize, source.Dpi, roi.Width * bytesPerPixel);
                return bitmap;
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to crop bitmap", ex);
        }

    }

    //private static readonly Dictionary<PixelFormat, SKColorType> ColorTypeMap =
    //    new()
    //    {
    //        [PixelFormat.Bgra8888] = SKColorType.Bgra8888
    //    };

    //public static byte[] EncodeThumbnailJpeg(Bitmap bitmap, int width, int height, int quality)
    //{
    //    var resized = ThumbnailBitmapFrom(bitmap, width, height);
    //    return EncodeToJpeg(resized, quality);
    //}

    //public static byte[] EncodeToJpeg(this Bitmap bitmap, int quality = 80)
    //{
    //    if (bitmap is not WriteableBitmap writeableBitmap)
    //    {
    //        writeableBitmap = WriteableFromBitmap(bitmap);
    //    }

    //    if (writeableBitmap is null)
    //    {
    //        return [];
    //    }

    //    try
    //    {
    //        using ILockedFramebuffer frameBuffer = writeableBitmap.Lock();
    //        SKColorType colorType = ColorTypeMap[bitmap.Format!.Value];
    //        var skImageInfo = new SKImageInfo(frameBuffer.Size.Width, frameBuffer.Size.Height, colorType);
    //        using var skBitmap = new SKBitmap(skImageInfo);
    //        skBitmap.InstallPixels(skImageInfo, frameBuffer.Address, frameBuffer.RowBytes);
    //        using var skImage = SKImage.FromBitmap(skBitmap);
    //        return skImage.Encode(SKEncodedImageFormat.Jpeg, quality).ToArray();
    //    }
    //    finally
    //    {
    //        writeableBitmap.Dispose();
    //    }
    //}

    //public static unsafe WriteableBitmap Clahe(this WriteableBitmap sourceBitmap, float clipLimit, IProfiler profiler)
    //{
    //    try
    //    {
    //        using ILockedFramebuffer sourceFrameBuffer = sourceBitmap.Lock();

    //        // Define the source rectangle (e.g., the entire bitmap)
    //        int height = sourceFrameBuffer.Size.Height;
    //        int width = sourceFrameBuffer.Size.Width;
    //        PixelRect sourceRect = new(0, 0, width, height);
    //        byte[] imageBuffer = new byte[height * width * 4];
    //        fixed (byte* arrayPtr = imageBuffer)
    //        {
    //            // The 'dataArray' is pinned here, and 'arrayPtr' points to its first element.
    //            nint buffer = (nint)arrayPtr;
    //            sourceBitmap.CopyPixels(sourceRect, buffer, imageBuffer.Length, sourceFrameBuffer.RowBytes);
    //        }

    //        byte[] bytes;
    //        var clahe = new Clahe(8, 8, clipLimit);
    //        bytes = clahe.Process(imageBuffer, height, width, profiler);
    //        fixed (byte* arrayPtr = bytes)
    //        {
    //            // The 'dataArray' is pinned here, and 'arrayPtr' points to its first element.
    //            IntPtr data = (IntPtr)arrayPtr;
    //            var newBitmap = new WriteableBitmap(
    //                (PixelFormat)sourceBitmap.Format!,
    //                (AlphaFormat)sourceBitmap.AlphaFormat!,
    //                data,
    //                sourceBitmap.PixelSize,
    //                sourceBitmap.Dpi,
    //                sourceFrameBuffer.RowBytes);

    //            return newBitmap;
    //        }
    //    }
    //    catch ( Exception ex)
    //    {
    //        Debug.WriteLine("Failed: " + ex);
    //        throw new Exception("Failed to apply Clahe: " +  ex);
    //    }
    //}

    public static unsafe byte[] ImageBytes(this WriteableBitmap sourceBitmap)
    {
        try
        {
            using ILockedFramebuffer sourceFrameBuffer = sourceBitmap.Lock();

            // Define the source rectangle (e.g., the entire bitmap)
            int height = sourceFrameBuffer.Size.Height;
            int width = sourceFrameBuffer.Size.Width;
            PixelRect sourceRect = new(0, 0, width, height);
            byte[] imageBuffer = new byte[height * width * 4];
            fixed (byte* arrayPtr = imageBuffer)
            {
                // The 'dataArray' is pinned here, and 'arrayPtr' points to its first element.
                nint buffer = (nint)arrayPtr;
                sourceBitmap.CopyPixels(sourceRect, buffer, imageBuffer.Length, sourceFrameBuffer.RowBytes);
            }

            return imageBuffer;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ImageBytes Failed: " + ex);
            throw new Exception("Failed to retrieve ImageBytes: " + ex);
        }
    }
}


/*
        #region Resize

        /// <summary>
        /// Creates a new resized WriteableBitmap.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="width">The new desired width.</param>
        /// <param name="height">The new desired height.</param>
        /// <param name="interpolation">The interpolation method that should be used.</param>
        /// <returns>A new WriteableBitmap that is a resized version of the input.</returns>
        public static WriteableBitmap Resize(this WriteableBitmap bmp, int width, int height, Interpolation interpolation)
        {
            using (var srcContext = bmp.GetBitmapContext(ReadWriteMode.ReadOnly))
            {
                var pd = Resize(srcContext, srcContext.Width, srcContext.Height, width, height, interpolation);

                var result = BitmapFactory.New(width, height);
                using (var dstContext = result.GetBitmapContext())
                {
                    BitmapContext.BlockCopy(pd, 0, dstContext, 0, SizeOfArgb * pd.Length);
                }
                return result;
            }
        }

        /// <summary>
        /// Creates a new resized bitmap.
        /// </summary>
        /// <param name="srcContext">The source context.</param>
        /// <param name="widthSource">The width of the source pixels.</param>
        /// <param name="heightSource">The height of the source pixels.</param>
        /// <param name="width">The new desired width.</param>
        /// <param name="height">The new desired height.</param>
        /// <param name="interpolation">The interpolation method that should be used.</param>
        /// <returns>A new bitmap that is a resized version of the input.</returns>
        public static int[] Resize(BitmapContext srcContext, int widthSource, int heightSource, int width, int height, Interpolation interpolation)
        {
            return Resize(srcContext.Pixels, widthSource, heightSource, width, height, interpolation);
        }

        /// <summary>
        /// Creates a new resized bitmap.
        /// </summary>
        /// <param name="pixels">The source pixels.</param>
        /// <param name="widthSource">The width of the source pixels.</param>
        /// <param name="heightSource">The height of the source pixels.</param>
        /// <param name="width">The new desired width.</param>
        /// <param name="height">The new desired height.</param>
        /// <param name="interpolation">The interpolation method that should be used.</param>
        /// <returns>A new bitmap that is a resized version of the input.</returns>
#if WPF
        public static int[] Resize(int* pixels, int widthSource, int heightSource, int width, int height, Interpolation interpolation)
#else
      public static int[] Resize(int[] pixels, int widthSource, int heightSource, int width, int height, Interpolation interpolation)
#endif
        {
            var pd = new int[width * height];
            var xs = (float)widthSource / width;
            var ys = (float)heightSource / height;

            float fracx, fracy, ifracx, ifracy, sx, sy, l0, l1, rf, gf, bf;
            int c, x0, x1, y0, y1;
            byte c1a, c1r, c1g, c1b, c2a, c2r, c2g, c2b, c3a, c3r, c3g, c3b, c4a, c4r, c4g, c4b;
            byte a, r, g, b;

            // Nearest Neighbor
            if (interpolation == Interpolation.NearestNeighbor)
            {
                var srcIdx = 0;
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        sx = x * xs;
                        sy = y * ys;
                        x0 = (int)sx;
                        y0 = (int)sy;

                        pd[srcIdx++] = pixels[y0 * widthSource + x0];
                    }
                }
            }

               // Bilinear
            else if (interpolation == Interpolation.Bilinear)
            {
                var srcIdx = 0;
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        sx = x * xs;
                        sy = y * ys;
                        x0 = (int)sx;
                        y0 = (int)sy;

                        // Calculate coordinates of the 4 interpolation points
                        fracx = sx - x0;
                        fracy = sy - y0;
                        ifracx = 1f - fracx;
                        ifracy = 1f - fracy;
                        x1 = x0 + 1;
                        if (x1 >= widthSource)
                        {
                            x1 = x0;
                        }
                        y1 = y0 + 1;
                        if (y1 >= heightSource)
                        {
                            y1 = y0;
                        }


                        // Read source color
                        c = pixels[y0 * widthSource + x0];
                        c1a = (byte)(c >> 24);
                        c1r = (byte)(c >> 16);
                        c1g = (byte)(c >> 8);
                        c1b = (byte)(c);

                        c = pixels[y0 * widthSource + x1];
                        c2a = (byte)(c >> 24);
                        c2r = (byte)(c >> 16);
                        c2g = (byte)(c >> 8);
                        c2b = (byte)(c);

                        c = pixels[y1 * widthSource + x0];
                        c3a = (byte)(c >> 24);
                        c3r = (byte)(c >> 16);
                        c3g = (byte)(c >> 8);
                        c3b = (byte)(c);

                        c = pixels[y1 * widthSource + x1];
                        c4a = (byte)(c >> 24);
                        c4r = (byte)(c >> 16);
                        c4g = (byte)(c >> 8);
                        c4b = (byte)(c);


                        // Calculate colors
                        // Alpha
                        l0 = ifracx * c1a + fracx * c2a;
                        l1 = ifracx * c3a + fracx * c4a;
                        a = (byte)(ifracy * l0 + fracy * l1);

                        // Red
                        l0 = ifracx * c1r + fracx * c2r;
                        l1 = ifracx * c3r + fracx * c4r;
                        rf = ifracy * l0 + fracy * l1;

                        // Green
                        l0 = ifracx * c1g + fracx * c2g;
                        l1 = ifracx * c3g + fracx * c4g;
                        gf = ifracy * l0 + fracy * l1;

                        // Blue
                        l0 = ifracx * c1b + fracx * c2b;
                        l1 = ifracx * c3b + fracx * c4b;
                        bf = ifracy * l0 + fracy * l1;

                        // Cast to byte
                        r = (byte)rf;
                        g = (byte)gf;
                        b = (byte)bf;

                        // Write destination
                        pd[srcIdx++] = (a << 24) | (r << 16) | (g << 8) | b;
                    }
                }
            }
            return pd;
        }

        #endregion
*/