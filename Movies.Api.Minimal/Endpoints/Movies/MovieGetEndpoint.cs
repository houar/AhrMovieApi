using Movies.Api.Minimal.Auth;
using Movies.Api.Minimal.Mapping;
using Movies.Application.Services;

namespace Movies.Api.Minimal.Endpoints.Movies
{
    internal static class MovieGetEndpoint
    {
        internal const string Name = "GetMovie";

        internal static void MapGetMovie(this IEndpointRouteBuilder app)
        {
            app.MapGet(ApiEndpoints.Movies.Get, async (string idOrSlug
                , IMovieService movieService
                , HttpContext context
                , CancellationToken token) =>
            {
                var userId = context.GetUserId();
                var movie = Guid.TryParse(idOrSlug, out var id)
                    ? await movieService.GetByIdAsync(id, userId, token)
                    : await movieService.GetBySlugAsync(idOrSlug, userId, token);
                if (movie == null)
                {
                    return Results.NotFound();
                }
                var response = movie.MapToMovieResponse();
                return TypedResults.Ok(response);
            })
                .WithName(Name)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization();
        }
    }
}
