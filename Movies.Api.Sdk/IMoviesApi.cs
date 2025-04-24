using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses.V1;
using Movies.Contracts.Responses.V2;
using Refit;

namespace Movies.Api.Sdk
{
    //[Headers("x-api-version: 2.0")]
    public interface IMoviesApi
    {
        [Get(ApiEndpoints.Movies.Get)]
        Task<MovieResponseV2> GetMovieAsync(string idOrSlug);

        [Headers("x-api-version: 2.0")]
        [Get(ApiEndpoints.Movies.Get)]
        Task<MovieResponseV2> GetMovieApiVersionHeaderAsync(string idOrSlug);

        [Get(ApiEndpoints.Movies.Get)]
        Task<MovieResponseV2> GetMovieApiVersionQueryAsync([Query] QueryParams apiVersion, string idOrSlug);

        [Get(ApiEndpoints.Movies.GetAll)]
        Task<MovieResponseV2> GetAllMoviesAsync(string idOrSlug);

        [Put(ApiEndpoints.Movies.Update)]
        Task<ApiResponse<MovieResponse>> UpdateMovie(Guid id, MovieReqUpdate movieReq);
    }

    public class QueryParams
    {
        [AliasAs("api-version")]
        public string apiversion { get; set; }
    }
}
