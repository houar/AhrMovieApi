using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieRepository.GetMoviesAsync();
            return Ok(movies.MapToMoviesResponse());
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> GetMovie(Guid id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie.MapToMovieResponse());
        }

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> AddMovie([FromBody] MovieReqCreate movieReq)
        {
            var movie = movieReq.MapToMovie();
            var created = await _movieRepository.CreateAsync(movie);
            if (created == false)
            {
                return StatusCode(500, "An error occurred while creating the movie.");
            }
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie.MapToMovieResponse());
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> UpdateMovie([FromRoute] Guid id, [FromBody] MovieReqUpdate movieReq)
        {
            var movie = movieReq.MapToMovie(id);
            var updated = await _movieRepository.UpdateAsync(movie);
            if (updated == false)
            {
                return NotFound();
            }
            return Ok(movie.MapToMovieResponse());
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteMovie(int id)
        //{
        //    var deletedMovie = await _moviesRepository.DeleteMovie(id);
        //    if (deletedMovie == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(deletedMovie);
        //}
    }
}
