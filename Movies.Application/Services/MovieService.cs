using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IValidator<Movie> _movieValidator;
        private readonly IRatingRepository _ratingRepository;
        private readonly IValidator<GetAllMoviesOptions> _optionsValidator;

        public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator, IRatingRepository ratingRepository, IValidator<GetAllMoviesOptions> optionsValidator)
        {
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
            _movieValidator = movieValidator ?? throw new ArgumentNullException(nameof(movieValidator));
            _ratingRepository = ratingRepository ?? throw new ArgumentNullException(nameof(ratingRepository));
            _optionsValidator = optionsValidator ?? throw new ArgumentNullException(nameof(optionsValidator));
        }

        public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, token);
            return await _movieRepository.CreateAsync(movie, token);
        }

        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _movieRepository.DeleteByIdAsync(id, token);
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _movieRepository.ExistsByIdAsync(id, token);
        }

        public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
        {
            return await _movieRepository.GetByIdAsync(id, userId, token);
        }

        public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
        {
            return await _movieRepository.GetBySlugAsync(slug, userId, token);
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync(GetAllMoviesOptions options, CancellationToken token = default)
        {
            await _optionsValidator.ValidateAndThrowAsync(options, token);
            return await _movieRepository.GetMoviesAsync(options, token);
        }

        public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, token);
            var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, token);
            if (movieExists == false)
            {
                return null;
            }

            await _movieRepository.UpdateAsync(movie, token);

            if (userId.HasValue == false)
            {
                movie.Rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
                return movie;
            }

            var (rating, userRating) = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
            movie.Rating = rating;
            movie.UserRating = userRating;

            return movie;
        }
    }
}
