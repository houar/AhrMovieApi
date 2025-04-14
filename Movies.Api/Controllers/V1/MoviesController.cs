using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;
using Movies.Contracts.Responses.V1;

namespace Movies.Api.Controllers.V1
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiController]
    public partial class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
        }

        [Authorize]
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMovies([FromQuery] GetAllMoviesRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var options = request.MapToOptions().WithUser(userId);
            var movies = await _movieService.GetMoviesAsync(options, token);
            var total = await _movieService.GetCountAsync(request.Title, request.Year, token);
            return Ok(movies.MapToMoviesResponse(request.Page, request.PageSize, total));
        }

        [Authorize]
        [HttpGet(ApiEndpoints.Movies.Get)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMovie([FromRoute] string idOrSlug
            , [FromServices] LinkGenerator linkGenerator
            , [FromQuery] bool includeLinks
            , CancellationToken token)
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
            if (includeLinks)
            {
                response.AddLink(new HalLink()
                {
                    Href = linkGenerator.GetUriByAction(httpContext: HttpContext, action: nameof(GetMovie), values: new { idOrSlug = movie.Slug })!,
                    Rel = "self",
                    Type = "GET"
                });
                response.AddLink(new HalLink()
                {
                    Href = linkGenerator.GetUriByAction(httpContext: HttpContext, action: nameof(UpdateMovie), values: new { id = movie.Id })!,
                    Rel = "self",
                    Type = "PUT"
                });
                response.AddLink(new HalLink()
                {
                    Href = linkGenerator.GetUriByAction(httpContext: HttpContext, action: nameof(DeleteMovie), values: new { id = movie.Id })!,
                    Rel = "self",
                    Type = "DELETE"
                });
            }
            return Ok(response);
        }

        [Authorize(AuthConstants.AdminOrTrustedPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
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

        [Authorize(AuthConstants.AdminOrTrustedPolicyName)]
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
            return Ok(updated.MapToMovieResponse());
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
