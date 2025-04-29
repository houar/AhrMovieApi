using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Minimal.Auth;
using Movies.Application.Services;

namespace Movies.Api.Minimal.Endpoints.Movies
{
    internal static class MovieDeleteEndpoint
    {
        internal const string Name = "DeleteMovie";

        internal static void MapDeleteMovie(this IEndpointRouteBuilder app)
        {
            app.MapDelete(ApiEndpoints.Movies.Delete, async (Guid id
                , IMovieService movieService
                , IOutputCacheStore outputCacheStore
                , CancellationToken token) =>
            {
                var deleted = await movieService.DeleteByIdAsync(id, token);
                if (deleted == false)
                {
                    return Results.NotFound();
                }
                await outputCacheStore.EvictByTagAsync("movie-get-all", token);
                return Results.NoContent();
            })
                .WithName(Name)
                .RequireAuthorization(AuthConstants.AdminUserPolicyName);
        }
    }
}
