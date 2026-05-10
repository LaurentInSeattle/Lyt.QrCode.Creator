#if DEBUG 
// #define DEBUG_BUFFER_POOL
#endif

namespace Lyt.VideoCapture.Buffers;

public sealed class BufferPool(int maxReservedBufferElements)
{
    private sealed class Buffer(byte[] buffer)
    {
        private readonly int size = buffer.Length;

        private readonly WeakReference wr = new(buffer);

        public bool IsAvailable => this.wr.IsAlive;

        public bool IsAvailableAndFit(int minimumSize) => this.wr.IsAlive && (minimumSize <= this.size);

        public byte[]? ExtractBuffer() => (byte[]?)this.wr.Target;
    }

    private readonly Buffer?[] buffers = new Buffer?[maxReservedBufferElements];

    public BufferPool() : this(16) { }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int UnsafeAvailableCount =>
        this.buffers.Count(bufferHolder => bufferHolder?.IsAvailable ?? false);

    public byte[] Borrow(int minimumSize)
    {
        for (int index = 0; index < this.buffers.Length; index++)
        {
            var bufferElement = this.buffers[index];

            // First phase:
            // * Determined: size and exactSize
            // * NOT determined: Availability
            if (bufferElement?.IsAvailableAndFit(minimumSize) ?? false)
            {
                if (object.ReferenceEquals(
                    Interlocked.CompareExchange(ref this.buffers[index], null, bufferElement),
                    bufferElement) &&
                    // Second phase
                    // * Determined: size, exactSize and availability
                    bufferElement.ExtractBuffer() is { } buffer)
                {
#if DEBUG_BUFFER_POOL
                    Debug.WriteLine($"DefaultBufferPool: Rent: Size={buffer.Length}/{minimumSize}, Index={index}");
#endif
                    return buffer;
                }
            }
            else if (!(bufferElement?.IsAvailable ?? true))
            {
                // Remove corrected element (and forgot).
                Interlocked.CompareExchange(ref this.buffers[index], null, bufferElement);
            }
        }

#if DEBUG_BUFFER_POOL
        Debug.WriteLine($"DefaultBufferPool: Created: Size={minimumSize}");
#endif
        return new byte[minimumSize];
    }

    public void Return(byte[] buffer)
    {
        Buffer newBufferElement = new(buffer);
        for (int index = 0; index < this.buffers.Length; index++)
        {
            var bufferElement = this.buffers[index];
            if (bufferElement == null || !bufferElement.IsAvailable)
            {
                if (object.ReferenceEquals(
                    Interlocked.CompareExchange(ref this.buffers[index], newBufferElement, bufferElement),
                    bufferElement))
                {
#if DEBUG_BUFFER_POOL
                    Debug.WriteLine($"DefaultBufferPool: Returned: Size={buffer.Length}, Index={index}");
#endif
                    return;
                }
            }
        }

        // It was better to simply discard a buffer instance than the cost of extending the table.
#if DEBUG_BUFFER_POOL
        Debug.WriteLine($"DefaultBufferPool: Discarded: Size={buffer.Length}");
#endif
    }
}
