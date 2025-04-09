using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetMovies(CancellationToken token)
        {
            var movies = await _movieService.GetMoviesAsync(token);
            return Ok(movies.MapToMoviesResponse());
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> GetMovie([FromRoute] string idOrSlug, CancellationToken token)
        {
            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await _movieService.GetByIdAsync(id, token)
                : await _movieService.GetBySlugAsync(idOrSlug, token);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie.MapToMovieResponse());
        }

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> AddMovie([FromBody] MovieReqCreate movieReq, CancellationToken token)
        {
            var movie = movieReq.MapToMovie();
            var created = await _movieService.CreateAsync(movie, token);
            if (created == false)
            {
                return StatusCode(500, "An error occurred while creating the movie.");
            }
            return CreatedAtAction(nameof(GetMovie), new { idOrSlug = movie.Id }, movie.MapToMovieResponse());
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> UpdateMovie([FromRoute] Guid id, [FromBody] MovieReqUpdate movieReq, CancellationToken token)
        {
            var movie = movieReq.MapToMovie(id);
            var updated = await _movieService.UpdateAsync(movie, token);
            if (updated == null)
            {
                return NotFound();
            }
            return Ok(updated.MapToMovieResponse());
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> DeleteMovie([FromRoute] Guid id, CancellationToken token)
        {
            var deleted = await _movieService.DeleteByIdAsync(id, token);
            if (deleted == false)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
