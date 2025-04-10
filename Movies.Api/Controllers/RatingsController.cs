using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{
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
    }
}
