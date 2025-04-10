namespace Movies.Contracts.Responses
{
    public class RatingResponse
    {
        public required Guid MovieId { get; init; }
        public required string MovieSlug { get; init; }
        public required string MovieTitle { get; init; }
        public required int UserRating { get; init; }
    }
}
