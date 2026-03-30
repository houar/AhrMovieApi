using Movies.Api.Minimal.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;

namespace Movies.Api.Minimal.Endpoints.Ratings
{
    internal static class RatingRateMovie
    {
        internal const string Name = "RateMovie";

        internal static void MapRateMovie(this IEndpointRouteBuilder app)
        {
            app.MapPut(ApiEndpoints.Movies.Rate, async (Guid id
                , RatingRequest request
                , HttpContext context
                , IRatingService ratingService
                , CancellationToken token) =>
            {
                var userId = context.GetUserId();
                if (userId is null || userId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }
                var rating = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);
                return rating ? Results.Ok() : Results.NotFound();
            })
                .WithName(Name)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithApiVersionSet(ApiVersioning.VersionSet)
                .RequireAuthorization();
        }
    }
}
