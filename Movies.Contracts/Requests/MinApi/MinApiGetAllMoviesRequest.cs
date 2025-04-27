using Movies.Contracts.Requests.MinApi;

namespace Movies.Contracts.Requests.MinApi
{
    public class MinApiGetAllMoviesRequest : MinApiPagedRequest
    {
        public string? Title { get; init; }
        public int? Year { get; init; }
        public string? SortBy { get; init; }
    }
}
