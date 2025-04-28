using Movies.Api.Minimal.Auth;
using Movies.Api.Minimal.Mapping;
using Movies.Application.Services;

namespace Movies.Api.Minimal.Endpoints.Ratings
{
    internal static class RatingGetAllForUserEndpoint
    {
        internal const string Name = "GetUserRatings";

        internal static void MapGetUserRatings(this IEndpointRouteBuilder app)
        {
            app.MapGet(ApiEndpoints.Ratings.GetUserRatings, async (HttpContext context
                , IRatingService ratingService
                , CancellationToken token) =>
            {
                var userId = context.GetUserId();
                if (userId is null || userId == Guid.Empty)
                {
                    return Results.Unauthorized();
                }
                var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);
                return TypedResults.Ok(ratings.MapToRatingsResponse());
            })
            .WithName(Name);
        }
    }
}
