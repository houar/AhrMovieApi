using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IMovieRepository _movieRepository;

        public RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository)
        {
            _ratingRepository = ratingRepository ?? throw new ArgumentNullException(nameof(ratingRepository));
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
        }

        public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
        {
            if (rating < 1 || rating > 5)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(rating), "Rating must be between 1 and 5.")
                });
            }

            var movieExists = await _movieRepository.ExistsByIdAsync(movieId, token);
            if (movieExists == false)
            {
                return false;
            }

            return await _ratingRepository.RateMovieAsync(movieId, rating, userId, token);
        }
    }
}
