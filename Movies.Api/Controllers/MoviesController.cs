using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
        }

        //[HttpGet]
        //public async Task<IActionResult> GetMovies()
        //{
        //    var movies = await _moviesRepository.GetMovies();
        //    return Ok(movies);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(Guid id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] MovieReqCreate movieReq)
        {
            var movie = movieReq.MapToMovie();
            var created = await _movieRepository.CreateAsync(movie);
            if (created == false)
            {
                return StatusCode(500, "An error occurred while creating the movie.");
            }
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateMovie(int id, [FromBody] Movie movie)
        //{
        //    var updatedMovie = await _moviesRepository.UpdateMovie(id, movie);
        //    if (updatedMovie == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(updatedMovie);
        //}

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
