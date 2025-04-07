using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators
{
    public class MovieValidator : AbstractValidator<Movie>
    {
        private readonly IMovieRepository _movieRepository;

        public MovieValidator(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;

            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty();

            RuleFor(x => x.Genres)
                .NotEmpty();

            RuleFor(x => x.YearOfRelease)
                .LessThanOrEqualTo(DateTime.Now.Year);

            RuleFor(x => x.Slug)
                .MustAsync(ValidateSlug)
                .WithMessage("This movie already exists in the application");
        }

        private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken cancellationToken = default)
        {
            var existingMovie = await _movieRepository.GetBySlugAsync(slug);
            // Check if the movie is being updated
            if (existingMovie is not null)
            {
                return movie.Id == existingMovie.Id;
            };
            // If the movie is not being updated (it is being created), check if it already exists
            // (at this stage we already know it does not exist)
            return true;
        }
    }
}
