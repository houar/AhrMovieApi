using Movies.Api.Minimal.Auth;
using Movies.Api.Minimal.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.MinApi;

namespace Movies.Api.Minimal.Endpoints.Movies
{
    internal static class MovieGetAllEndpoint
    {
        internal const string Name = "GetAllMovies";

        internal static void MapGetAllMovies(this IEndpointRouteBuilder app)
        {
            app.MapGet(ApiEndpoints.Movies.GetAll, async ([AsParameters] MinApiGetAllMoviesRequest request
                , IMovieService movieService
                , HttpContext context
                , CancellationToken token) =>
            {
                var userId = context.GetUserId();
                var options = request.MapToOptions().WithUser(userId);
                var movies = await movieService.GetMoviesAsync(options, token);
                var total = await movieService.GetCountAsync(request.Title, request.Year, token);
                return TypedResults.Ok(movies.MapToMoviesResponse(request.Page.GetValueOrDefault(MinApiPagedRequest.DefaultPage)
                    , request.PageSize.GetValueOrDefault(MinApiPagedRequest.DefaultPageSize), total));
            })
                .WithName(Name);
        }
    }
}
