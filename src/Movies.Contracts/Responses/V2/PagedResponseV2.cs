namespace Movies.Contracts.Responses.V2
{
    public class PagedResponseV2<TResponse>
    {
        public required int PageSize { get; init; }
        public required int Page { get; init; }
        public required int TotalCount { get; init; }
        public bool HasNextPage => Page * PageSize < TotalCount;
        public string ApiVersion { get; init; } = "VERSION-2.0";
        public required IEnumerable<TResponse> Items { get; init; } = Enumerable.Empty<TResponse>();
    }
}
