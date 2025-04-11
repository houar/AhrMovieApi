namespace Movies.Contracts.Responses
{
    public class PagedResponse<TResponse>
    {
        public required int PageSize { get; init; }
        public required int Page { get; init; }
        public required int TotalCount { get; init; }
        public bool HasNextPage => Page * PageSize < TotalCount;
        public required IEnumerable<TResponse> Items { get; init; } = Enumerable.Empty<TResponse>();
    }
}
