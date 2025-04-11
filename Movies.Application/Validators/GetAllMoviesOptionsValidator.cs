using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators
{
    public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
    {
        private static readonly string[] AllowedSortFields = { "title", "year" };

        public GetAllMoviesOptionsValidator()
        {
            RuleFor(x => x.YearOfRelease)
                .LessThanOrEqualTo(DateTime.UtcNow.Year)
                .WithMessage($"Year must be less than or equal to {DateTime.UtcNow.Year}.");

            RuleFor(x => x.SortField)
                .Must(x => string.IsNullOrEmpty(x) || AllowedSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Sort field have be only one of the following: {string.Join(", ", AllowedSortFields)}.");
        }
    }
}
