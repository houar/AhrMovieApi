using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V2;
using Movies.Contracts.Responses.V1;
using Movies.Contracts.Responses.V2;

namespace Movies.Api.Controllers.V3
{
    [ApiVersion("3.0")]
    [ApiController]
    public partial class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
        }

        [ServiceFilter(typeof(IAsyncAuthorizationFilter))]
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        [OutputCache(PolicyName = "MovieGetAllV3")]
        [ProducesResponseType(typeof(MoviesResponseV2), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMovies([FromQuery] GetAllMoviesRequestV2 request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var options = request.MapToOptions().WithUserV2(userId);
            var movies = await _movieService.GetMoviesAsync(options, token);
            var total = await _movieService.GetCountAsync(request.Title, request.Year, token);
            return Ok(movies.MapToMoviesResponseV2(request.Page, request.PageSize, total));
        }

        [ServiceFilter(typeof(IAsyncAuthorizationFilter))]
        [ResponseCache(Duration = 60, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
        [OutputCache(PolicyName = "MovieGetWithUserRatV3", Duration = 60)]
        [HttpGet(ApiEndpoints.Movies.Get)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMovie([FromRoute] string idOrSlug, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await _movieService.GetByIdAsync(id, userId, token)
                : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
            if (movie == null)
            {
                return NotFound();
            }
            var response = movie.MapToMovieResponse();
            return Ok(response);
        }
    }
}
