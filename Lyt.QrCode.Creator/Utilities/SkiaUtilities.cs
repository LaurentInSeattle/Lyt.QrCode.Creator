namespace Lyt.QrCode.Creator.Utilities;

using SkiaSharp;

using global::Avalonia.Skia;
using global::Avalonia.Rendering.SceneGraph;

public static class SkiaExtensions
{
    private record class SKBitmapDrawOperation : ICustomDrawOperation
    {
        public Rect Bounds { get; set; }

        public SKBitmap? Bitmap { get; init; }

#pragma warning disable CA1816 
        // Dispose methods should call SuppressFinalize
        public void Dispose() { /* nop */ }
#pragma warning restore CA1816 

        public bool Equals(ICustomDrawOperation? other) => false;

        public bool HitTest(Point p) => this.Bounds.Contains(p);

        public void Render(ImmediateDrawingContext context)
        {
            if (this.Bitmap is SKBitmap bitmap && 
                context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
            {
                ISkiaSharpApiLease lease = leaseFeature.Lease();
                using (lease)
                {
                    lease.SkCanvas.DrawBitmap(
                        bitmap, 
                        SKRect.Create(
                            (float)this.Bounds.X, (float)this.Bounds.Y, (float)this.Bounds.Width, (float)this.Bounds.Height));
                }
            }
        }
    }

    public class AvaloniaImage : IImage, IDisposable
    {
        private readonly SKBitmap? source;

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
            source?.Dispose();
            drawImageOperation?.Dispose();
        } 

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
            if (drawImageOperation is null)
            {
                drawImageOperation = new SKBitmapDrawOperation() { Bitmap = source };
            }
            
            drawImageOperation.Bounds = sourceRect;
            context.Custom(drawImageOperation);
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
}