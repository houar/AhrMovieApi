using Movies.Api.Minimal.Auth;
using Movies.Application.Services;

namespace Movies.Api.Minimal.Endpoints.Ratings
{
    internal static class RatingDeleteEndpoint
    {
        internal const string Name = "DeleteRating";

        internal static void MapDeleteRating(this IEndpointRouteBuilder app)
        {
            app.MapDelete(ApiEndpoints.Movies.DeleteRating, async (Guid movieId
                , HttpContext context
                , IRatingService ratingService
                , CancellationToken token) =>
            {
                var userId = context.GetUserId();
                if (userId is null || userId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }
                var result = await ratingService.DeleteRatingAsync(movieId, userId!.Value, token);
                return result ? Results.NoContent() : Results.NotFound(new { Message = "Rating not found." });
            })
                .WithName(Name)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithApiVersionSet(ApiVersioning.VersionSet)
                .RequireAuthorization();
        }
    }
}
