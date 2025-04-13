namespace Movies.Contracts.Requests.V2
{
    public class GetAllMoviesRequestV2 : PagedRequestV2
    {
        public required string? Title { get; init; }
        public required int? Year { get; init; }
        public required string? SortBy { get; init; }
    }
}
