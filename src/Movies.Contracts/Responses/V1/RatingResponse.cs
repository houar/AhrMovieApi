namespace Movies.Contracts.Responses.V1
{
    public class RatingResponse
    {
        public required Guid MovieId { get; init; }
        public required string MovieSlug { get; init; }
        public required string MovieTitle { get; init; }
        public required int UserRating { get; init; }
    }
}
