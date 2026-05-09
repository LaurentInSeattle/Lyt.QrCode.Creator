namespace Lyt.VideoCapture.Threading;

internal sealed class AsyncManualResetEvent
{
    private volatile TaskCompletionSource<bool>? tcs;

    public void Set()
    {
        var tcs = Interlocked.Exchange( ref this.tcs, null);
        tcs?.TrySetResult(true);
    }

    public void Reset()
    {
        if (this.tcs == null)
        {
            Interlocked.CompareExchange( ref this.tcs, new TaskCompletionSource<bool>(), null);
        }
    }

    public async ValueTask WaitAsync(CancellationToken ct)
    {
        if (this.tcs is { } tcs)
        {
            using var _ = ct.Register(() => tcs.TrySetCanceled());
            await tcs.Task. ConfigureAwait(false);
        }
    }

    public static async ValueTask<int> WaitAnyAsync( CancellationToken ct, params AsyncManualResetEvent[] evs)
    {
        var captured = new Task[evs.Length];

        while (true)
        {
            for (int index = 0; index < captured.Length; index++)
            {
                if (evs[index].tcs is { } tcs)
                {
                    captured[index] = tcs.Task;
                }
                else
                {
                    return index;
                }
            }

            var result = await Task.WhenAny(captured).ConfigureAwait(false);
            for (int index = 0; index < captured.Length; index++)
            {
                if (object.ReferenceEquals(captured[index], result))
                {
                    return index;
                }
            }

            Debug.Assert(false);
        }
    }
}
