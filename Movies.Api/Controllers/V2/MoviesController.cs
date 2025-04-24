using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Requests.V2;
using Movies.Contracts.Responses;
using Movies.Contracts.Responses.V1;
using Movies.Contracts.Responses.V2;

namespace Movies.Api.Controllers.V2
{
    [Authorize]
    [ApiVersion("2.0")]
    [ApiController]
    public partial class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
        }

        [AllowAnonymous]//For testing purposes (Cache - from the browser)
        [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "title", "year", "sortBy", "page", "pageSize" }, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        [ProducesResponseType(typeof(MoviesResponseV2), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMovies([FromQuery] GetAllMoviesRequestV2 request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var options = request.MapToOptions().WithUserV2(userId);
            var movies = await _movieService.GetMoviesAsync(options, token);
            var total = await _movieService.GetCountAsync(request.Title, request.Year, token);
            return Ok(movies.MapToMoviesResponseV2(request.Page, request.PageSize, total));
        }

        [AllowAnonymous]//For testing purposes (Cache - from the browser)
        [ResponseCache(Duration = 60, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
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

        [AllowAnonymous]//For testing purposes (Cache - from the browser)
        //[Authorize(AuthConstants.MultiAuthPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMovie([FromRoute] Guid id, [FromBody] MovieReqUpdate movieReq, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var movie = movieReq.MapToMovie(id);
            var updated = await _movieService.UpdateAsync(movie, userId, token);
            if (updated == null)
            {
                return NotFound();
            }
            //await _outputCacheStore.EvictByTagAsync("movie-get-all", token);
            return Ok(updated.MapToMovieResponse());
        }
    }
}
