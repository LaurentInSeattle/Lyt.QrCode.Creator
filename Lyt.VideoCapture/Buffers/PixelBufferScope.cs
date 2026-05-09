namespace Lyt.VideoCapture.Buffers;

public abstract class PixelBufferScope
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected PixelBufferScope(PixelBuffer buffer) => this.Buffer = buffer;

    public PixelBuffer Buffer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void OnReleaseNow() => this.Buffer = null!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InternalReleaseNow() => this.OnReleaseNow();
}
