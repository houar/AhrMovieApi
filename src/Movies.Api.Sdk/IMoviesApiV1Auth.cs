using Movies.Contracts.Responses.V1;
using Refit;

namespace Movies.Api.Sdk
{
    public interface IMoviesApiV1Auth
    {
        [Headers("x-api-version: 1.0", "Authorization: Bearer")]
        [Get(ApiEndpoints.Movies.Get)]
        Task<MovieResponse> GetMovieV1NeedsAuthorizationAsync(string idOrSlug);
    }
}
