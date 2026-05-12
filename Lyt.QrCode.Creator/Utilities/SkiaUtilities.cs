namespace Lyt.QrCode.Creator.Utilities;

using SkiaSharp;

using global::Avalonia.Skia;
using global::Avalonia.Rendering.SceneGraph;

public static class SkiaExtensions
{
    private record class SKBitmapDrawOperation : ICustomDrawOperation
    {
        private bool isDisposed;

        public Rect Bounds { get; set; }

        public SKBitmap? Bitmap { get; set; }

        public void Dispose()
        {
            this.isDisposed = true;
            this.Bounds = new Rect();
            this.Bitmap?.Dispose();
            this.Bitmap = null;

            // Tells the GC not to call the finalizer; we've already cleaned up.
            GC.SuppressFinalize(this);
        }

        public bool Equals(ICustomDrawOperation? other) => false;

        public bool HitTest(Point p) => this.Bounds.Contains(p);

        public void Render(ImmediateDrawingContext context)
        {
            if (this.isDisposed)
            {
                if (Debugger.IsAttached)
                {
                    // Attempting to render a disposed operation: 
                    //Debugger.Break();
                }
            
                return;
            }

            if (this.Bitmap is SKBitmap bitmap &&
                context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
            {
                try
                {
                    if (bitmap.IsEmpty || bitmap.IsNull || !bitmap.ReadyToDraw || bitmap.Bytes.Length == 0)
                    {
                        if (Debugger.IsAttached)
                        {
                            // SKBitmap is not ready to draw: 
                            Debugger.Break();
                        }

                        return;
                    }

                    var skRect = SKRect.Create(
                        (float)this.Bounds.X, (float)this.Bounds.Y, (float)this.Bounds.Width, (float)this.Bounds.Height);
                    if ( skRect.IsEmpty || skRect.Width <= 0.0 || skRect.Height <= 0.0)
                    {
                        if (Debugger.IsAttached)
                        {
                            // Destination rect is invalid: 
                            Debugger.Break();
                        }

                        return;
                    }

                    using ISkiaSharpApiLease lease = leaseFeature.Lease();
                    var canvas = lease.SkCanvas;
                    if ( canvas.Context is null || canvas.Context.IsAbandoned)
                    {
                        if (Debugger.IsAttached)
                        {
                            // SkCanvas is not ready to draw: 
                            Debugger.Break();
                        }

                        return;
                    }

                    // Randomly crashes on DrawBitmap for no obvious reason 
                    lease.SkCanvas.DrawBitmap(bitmap, skRect); 
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to render SKBitmap: {ex}");
                }
            }
        }
    }

    public class AvaloniaImage : IImage, IDisposable
    {
        private SKBitmap? source;

        private SKBitmapDrawOperation? drawImageOperation;

        public AvaloniaImage(SKBitmap? source)
        {
            this.source = source;
            if (source?.Info.Size is SKSizeI size)
            {
                this.Size = new(size.Width, size.Height);
            }
        }

        public Size Size { get; }

        public void Dispose()
        {
            this.source?.Dispose();
            this.drawImageOperation?.Dispose();
            this.source = null;
            this.drawImageOperation = null;

            // Tells the GC not to call the finalizer; we've already cleaned up.
            GC.SuppressFinalize(this);
        }

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
            if (this.source is null)
            {
                return;
            }

            if (this.drawImageOperation is null)
            {
                this.drawImageOperation = new SKBitmapDrawOperation() { Bitmap = this.source };
            }

            this.drawImageOperation.Bounds = sourceRect;
            context.Custom(this.drawImageOperation);
        }
    }

    public static SKBitmap? ToSKBitmap(this Stream? stream)
    {
        if (stream is null)
        {
            return null;
        }

        return SKBitmap.Decode(stream);
    }

    public static IImage? ToAvaloniaIImage(this SKBitmap? bitmap)
    {
        if (bitmap is not null)
        {
            return new AvaloniaImage(bitmap);
        }

        return default;
    }

    public static WriteableBitmap ToWriteableBitmap(this SKBitmap skiaBitmap)
    {
        // Ensure the formats match; Skia often uses Rgba8888 or Bgra8888
        WriteableBitmap avaloniaBitmap =
            new(
                new PixelSize(skiaBitmap.Width, skiaBitmap.Height),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul);
        using (var lockedBuffer = avaloniaBitmap.Lock())
        {
            // Get pointers to the source and destination
            IntPtr sourcePtr = skiaBitmap.GetPixels();
            IntPtr destPtr = lockedBuffer.Address;

            // Determine size to copy
            int size = skiaBitmap.RowBytes * skiaBitmap.Height;

            // Perform the direct memory copy
            unsafe
            {
                Buffer.MemoryCopy(
                    sourcePtr.ToPointer(),
                    destPtr.ToPointer(),
                    lockedBuffer.RowBytes * avaloniaBitmap.PixelSize.Height,
                    size);
            }
        }

        return avaloniaBitmap;
    }
}