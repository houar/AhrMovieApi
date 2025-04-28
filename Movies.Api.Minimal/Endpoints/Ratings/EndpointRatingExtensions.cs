namespace Movies.Api.Minimal.Endpoints.Ratings
{
    internal static class EndpointRatingExtensions
    {
        internal static void MapRatingEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapRateMovie();
            app.MapDeleteRating();
            app.MapGetUserRatings();
        }
    }
}
