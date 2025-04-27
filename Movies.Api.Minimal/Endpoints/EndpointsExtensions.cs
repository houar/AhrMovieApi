using Movies.Api.Minimal.Endpoints.Movies;
using Movies.Api.Minimal.Endpoints.Ratings;

namespace Movies.Api.Minimal.Endpoints
{
    internal static class EndpointsExtensions
    {
        internal static void MapApiEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapMovieEndpoints();
            app.MapRatingEndpoints();
        }
    }
}
