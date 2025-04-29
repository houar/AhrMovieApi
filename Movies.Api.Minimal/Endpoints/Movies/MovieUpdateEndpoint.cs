using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Minimal.Auth;
using Movies.Api.Minimal.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;
using Movies.Contracts.Responses.V1;

namespace Movies.Api.Minimal.Endpoints.Movies
{
    internal static class MovieUpdateEndpoint
    {
        internal const string Name = "UpdateMovie";

        internal static void MapUpdateMovie(this IEndpointRouteBuilder app)
        {
            app.MapPut(ApiEndpoints.Movies.Update, async (Guid id
                , MovieReqUpdate movieReq
                , IMovieService movieService
                , IOutputCacheStore outputCacheStore
                , HttpContext context
                , CancellationToken token) =>
            {
                var userId = context.GetUserId();
                var movie = movieReq.MapToMovie(id);
                var updated = await movieService.UpdateAsync(movie, userId, token);
                if (updated == null)
                {
                    return Results.NotFound();
                }
                await outputCacheStore.EvictByTagAsync("movie-get-all", token);
                return TypedResults.Ok(updated.MapToMovieResponse());
            })
                .WithName(Name)
                .Produces(StatusCodes.Status201Created, typeof(MovieResponse))
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest, typeof(ValidationFailureResponse))
                .RequireAuthorization(AuthConstants.AdminOrTrustedPolicyName);
        }
    }
}
