namespace Movies.Contracts.Requests.V2
{
    public class PagedRequestV2
    {
        public required int Page { get; init; } = 1;
        public required int PageSize { get; init; } = 10;
    }
}
