﻿using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IValidator<Movie> _movieValidator;

        public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator)
        {
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
            _movieValidator = movieValidator ?? throw new ArgumentNullException(nameof(movieValidator));
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

        public async Task<IEnumerable<Movie>> GetMoviesAsync(Guid? userId = default, CancellationToken token = default)
        {
            return await _movieRepository.GetMoviesAsync(userId, token);
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
            return movie;
        }
    }
}
