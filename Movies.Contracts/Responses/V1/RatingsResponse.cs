namespace Movies.Contracts.Responses.V1
{
    public class RatingsResponse
    {
        public required IEnumerable<RatingResponse> Ratings { get; init; } = Enumerable.Empty<RatingResponse>();
    }
}
