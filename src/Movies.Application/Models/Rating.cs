namespace Movies.Application.Models
{
    public class Rating
    {
        public required Guid MovieId { get; init; }
        public required string MovieSlug { get; init; }
        public required string MovieTitle { get; init; }
        public required int UserRating { get; init; }
    }
}
