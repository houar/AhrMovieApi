﻿using Movies.Application.Models;

namespace Movies.Application.Services
{
    public interface IMovieService
    {
        Task<bool> CreateAsync(Movie movie, CancellationToken token = default);
        Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default);
        Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default);
        Task<IEnumerable<Movie>> GetMoviesAsync(GetAllMoviesOptions options, CancellationToken token = default);
        Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default);
        Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
        Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);
        Task<int> GetCountAsync(string? title, int? year, CancellationToken token = default);
    }
}
