namespace Lyt.VideoCapture.Utilities;

public static class Extensions
{
    public static Stream AsStream(this ArraySegment<byte> segment) =>
        segment.Array is { } ?
            new MemoryStream(segment.Array, segment.Offset, segment.Count) :
            new MemoryStream([]);

    public static Stream AsStream(this byte[]? data) 
        => data is { } ? new MemoryStream(data) : new MemoryStream([]);

    public static IEnumerable<U> Collect<T, U>(
        this IEnumerable<T> enumerable, Func<T, U?> selector)
    {
        foreach (var value in enumerable)
        {
            if (selector(value) is { } mapped)
            {
                yield return mapped;
            }
        }
    }

    public static IEnumerable<U> CollectWhile<T, U>(
        this IEnumerable<T> enumerable, Func<T, U?> selector)
    {
        foreach (var value in enumerable)
        {
            if (selector(value) is { } mapped)
            {
                yield return mapped;
            }
            else
            {
                break;
            }
        }
    }
}
