using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V2;

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

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetMovies([FromQuery] GetAllMoviesRequestV2 request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var options = request.MapToOptions().WithUserV2(userId);
            var movies = await _movieService.GetMoviesAsync(options, token);
            var total = await _movieService.GetCountAsync(request.Title, request.Year, token);
            return Ok(movies.MapToMoviesResponseV2(request.Page, request.PageSize, total));
        }
    }
}
