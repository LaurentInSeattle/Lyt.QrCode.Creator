namespace Lyt.VideoCapture.FrameProcessors;

public abstract class FrameProcessor(BufferPool bufferPool)
{
    private readonly BufferPool bufferPool = bufferPool;
    private readonly Stack<PixelBuffer> reserver = new();

    protected FrameProcessor() : this(new DefaultBufferPool())
    {
    }

    [Obsolete("Dispose method overriding is obsoleted. Switch OnDisposeAsync instead.", true)]
    protected virtual void Dispose() => throw new InvalidOperationException();

    protected abstract Task OnDisposeAsync();

    public async Task DisposeAsync()
    {
        try
        {
            await this.OnDisposeAsync().
                ConfigureAwait(false);
        }
        finally
        {
            lock (this.reserver)
            {
                this.reserver.Clear();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected PixelBuffer GetPixelBuffer()
    {
        PixelBuffer? buffer = null;
        lock (this.reserver)
        {
            if (this.reserver.Count >= 1)
            {
                buffer = this.reserver.Pop();
            }
        }

        if (buffer == null)
        {
            buffer = new PixelBuffer(this.bufferPool);
        }

        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReleasePixelBuffer(PixelBuffer buffer)
    {
        lock (this.reserver)
        {
            this.reserver.Push(buffer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void Capture(CaptureDevice captureDevice,
        IntPtr pData, int size,
        long timestampMicroseconds, long frameIndex,
        PixelBuffer buffer) =>
        captureDevice.InternalOnCapture(pData, size, timestampMicroseconds, frameIndex, buffer);

    public abstract void OnFrameArrived(
        CaptureDevice captureDevice,
        IntPtr pData, int size, long timestampMicroseconds, long frameIndex);

    protected sealed class AutoPixelBufferScope : PixelBufferScope, IDisposable
    {
        private FrameProcessor? parent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AutoPixelBufferScope(
                FrameProcessor parent,
                PixelBuffer buffer) :
                base(buffer) =>
                this.parent = parent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            lock (this)
            {
                if (this.parent is { } parent)
                {
                    var buffer = this.Buffer;
                    base.OnReleaseNow();
                    this.parent.ReleasePixelBuffer(buffer);
                    this.parent = null;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnReleaseNow() => this.Dispose();
    }
}
