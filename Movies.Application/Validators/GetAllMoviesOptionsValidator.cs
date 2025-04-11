using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators
{
    public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
    {
        public GetAllMoviesOptionsValidator()
        {
            RuleFor(x => x.YearOfRelease)
                .LessThanOrEqualTo(DateTime.UtcNow.Year)
                .WithMessage($"Year must be less than or equal to {DateTime.UtcNow.Year}.");
        }
    }
}
