using Movies.Contracts.Responses.V2;
using Refit;

namespace Movies.Api.Sdk
{
    [Headers("x-api-version: 3.0")]
    public interface IMoviesApiV3
    {
        [Get(ApiEndpoints.Movies.Get)]
        Task<MovieResponseV2> GetMovieAsync(string idOrSlug);
    }
}
