using Movies.Api.Auth;
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
                    //return Results.Unauthorized();
                    // Only for demo purposes
                    return Results.Problem(
                        title: "Unauthorized",
                        detail: "Unauthorized - The user ID is required (JWT with Authorization header).",
                        statusCode: StatusCodes.Status401Unauthorized);
                }
                var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);
                return TypedResults.Ok(ratings.MapToRatingsResponse());
            })
                .WithName(Name)
                // Only for demo purposes
                .AddEndpointFilter<ApiKeyAuthFilter>();
        }
    }
}
