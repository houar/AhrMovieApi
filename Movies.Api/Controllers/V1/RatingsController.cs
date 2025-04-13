using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;

namespace Movies.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService ?? throw new ArgumentNullException(nameof(ratingService));
        }

        [Authorize]
        [HttpPost(ApiEndpoints.Movies.Rate)]
        [HttpPut(ApiEndpoints.Movies.Rate)]
        public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RatingRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null || userId == Guid.Empty)
            {
                return Unauthorized();
            }
            var rating = await _ratingService.RateMovieAsync(id, request.Rating, userId!.Value , token);
            return rating ? Ok() : NotFound();
        }

        [Authorize]
        [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
        public async Task<IActionResult> DeleteRating([FromRoute] Guid movieId, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null || userId == Guid.Empty)
            {
                return Unauthorized();
            }
            var result = await _ratingService.DeleteRatingAsync(movieId, userId!.Value, token);
            return result ? Ok() : NotFound();
        }

        [Authorize]
        [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
        public async Task<IActionResult> GetUserRatings(CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null || userId == Guid.Empty)
            {
                return Unauthorized();
            }
            var ratings = await _ratingService.GetRatingsForUserAsync(userId!.Value, token);
            return Ok(ratings.MapToRatingsResponse());
        }
    }
}
