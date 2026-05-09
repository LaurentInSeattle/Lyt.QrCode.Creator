namespace Lyt.VideoCapture.Threading;

internal sealed class AsyncLock
{
    private readonly Disposer disposer;
    private readonly Queue<TaskCompletionSource<Disposer>> queue = new();
    private int count;

    public AsyncLock() => this.disposer = new(this);

    public ValueTask<Disposer> LockAsync(CancellationToken ct)
    {
        int count = Interlocked.Increment(ref this.count);
        Debug.Assert(count >= 1);

        if (count == 1)
        {
            return new(this.disposer);
        }

        var tcs = new TaskCompletionSource<Disposer>();
        var ctr = ct.Register(() => tcs.TrySetCanceled());

        tcs.Task.ContinueWith(_ =>
            ctr.Dispose(),
            TaskContinuationOptions.ExecuteSynchronously);

        lock (this.queue)
        {
            this.queue.Enqueue(tcs);
        }

        return new(tcs.Task);
    }

    private void Unlock()
    {
        while (true)
        {
            int count = Interlocked.Decrement(ref this.count);
            Debug.Assert(count >= 0);

            if (count == 0)
            {
                break;
            }
            else if (count >= 1)
            {
                lock (this.queue)
                {
                    Debug.Assert(this.queue.Count >= 1);
                    var tcs = this.queue.Dequeue();
                    if (tcs.TrySetResult(this.disposer))
                    {
                        break;
                    }
                }
            }
        }
    }

    public sealed class Disposer : IDisposable
    {
        private readonly AsyncLock parent;

        internal Disposer(AsyncLock parent) => this.parent = parent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => this.parent.Unlock();
    }
}
