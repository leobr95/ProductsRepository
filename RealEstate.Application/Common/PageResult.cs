namespace RealEstate.Application.Common;

public class PagedResult<T>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public long Total { get; init; }
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
}
