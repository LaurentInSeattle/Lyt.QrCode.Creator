namespace Lyt.VideoCapture.Devices;

public abstract class CaptureDevice(object identity, string name) : IAsyncDisposable, IDisposable
{
    private readonly AsyncLock locker = new();

    public object Identity { get; } = identity;

    public string Name { get; } = name;

    public virtual bool HasPropertyPage => false;

    protected abstract Task OnInitializeAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        FrameProcessor frameProcessor,
        CancellationToken ct);

    protected abstract Task OnStartAsync(CancellationToken ct);

    protected abstract Task OnStopAsync(CancellationToken ct);

    protected abstract void OnCapture(
        IntPtr pData, int size, long timestampMicroseconds, long frameIndex, PixelBuffer buffer);

    protected virtual Task OnDisposeAsync() => Task.CompletedTask;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task StartAsync(CancellationToken ct = default) => this.InternalStartAsync(ct);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task StopAsync(CancellationToken ct = default) => this.InternalStopAsync(ct);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<bool> ShowPropertyPageAsync(IntPtr parentWindow, CancellationToken ct = default) 
        => this.InternalShowPropertyPageAsync(parentWindow, ct);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => _ = this.DisposeAsync().ConfigureAwait(false);

    ValueTask IAsyncDisposable.DisposeAsync() => new(this.DisposeAsync());

    public async Task DisposeAsync()
    {
        using var _ = await locker.LockAsync(default).ConfigureAwait(false);
        await this.OnDisposeAsync().ConfigureAwait(false);
    }

    public VideoCharacteristics Characteristics { get; protected set; } = null!;

    public bool IsRunning { get; protected set; }

    protected virtual Task<bool> OnShowPropertyPageAsync(
        IntPtr parentWindow, CancellationToken ct) =>
        Task.FromResult(false);


    internal Task InternalInitializeAsync(
        VideoCharacteristics characteristics,
        TranscodeFormats transcodeFormat,
        FrameProcessor frameProcessor,
        CancellationToken ct) =>
        this.OnInitializeAsync(characteristics, transcodeFormat, frameProcessor, ct);

    internal async Task InternalStartAsync(CancellationToken ct)
    {
        using var _ = await locker.LockAsync(ct).ConfigureAwait(false);
        await this.OnStartAsync(ct);
    }

    internal async Task InternalStopAsync(CancellationToken ct)
    {
        using var _ = await locker.LockAsync(ct).ConfigureAwait(false);
        await this.OnStopAsync(ct);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InternalOnCapture(IntPtr pData, int size, long timestampMicroseconds, long frameIndex, PixelBuffer buffer) 
        => this.OnCapture(pData, size, timestampMicroseconds, frameIndex, buffer);

    internal async Task<bool> InternalShowPropertyPageAsync(
        IntPtr parentWindow, CancellationToken ct)
    {
        using var _ = await locker.LockAsync(ct).ConfigureAwait(false);
        return await this.OnShowPropertyPageAsync(parentWindow, ct);
    }
}
