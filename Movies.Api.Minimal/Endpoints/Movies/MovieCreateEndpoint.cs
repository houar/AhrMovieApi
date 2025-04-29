using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Minimal.Auth;
using Movies.Api.Minimal.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;
using Movies.Contracts.Responses.V1;

namespace Movies.Api.Minimal.Endpoints.Movies
{
    internal static class MovieCreateEndpoint
    {
        internal const string Name = "CreateMovie";

        internal static void MapCreateMovie(this IEndpointRouteBuilder routes)
        {
            routes.MapPost(ApiEndpoints.Movies.Create, async (MovieReqCreate movieReq
                , IMovieService movieService
                , IOutputCacheStore outputCacheStore
                , CancellationToken token) =>
            {
                var movie = movieReq.MapToMovie();
                var created = await movieService.CreateAsync(movie, token);
                if (created == false)
                {
                    return Results.Problem(
                        detail: "Something went wrong!",
                        statusCode: 500
                    );
                }
                await outputCacheStore.EvictByTagAsync("movie-get-all", token);
                return TypedResults.CreatedAtRoute(movie.MapToMovieResponse(), MovieGetEndpoint.Name, new { idOrSlug = movie.Id });
            })
                .WithName(Name)
                .Produces(StatusCodes.Status201Created, typeof(MovieResponse))
                .Produces(StatusCodes.Status500InternalServerError)
                .Produces(StatusCodes.Status400BadRequest, typeof(ValidationFailureResponse))
                .RequireAuthorization(AuthConstants.AdminOrTrustedPolicyName);
        }
    }
}
