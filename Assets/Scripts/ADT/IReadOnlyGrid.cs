public interface IReadOnlyGrid<T>
{
    int Width { get; }
    int Height { get; }
    T this[int x, int y] { get; }
}