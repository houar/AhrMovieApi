using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MovieRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
        }

        public async Task<bool> CreateAsync(Movie movie)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();
            var inserted = await connection.ExecuteAsync(new CommandDefinition("INSERT INTO Movies" +
                " (id, slug, title, yearofrelease)" +
                " values (@Id, @Slug, @Title, @YearOfRelease)", movie));
            if (inserted > 0)
            {
                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(new CommandDefinition("INSERT INTO Genres" +
                        " (movieId, name)" +
                        " values (@MovieId, @Name)", new { MovieId = movie.Id, Name = genre }));
                }
            }
            transaction.Commit();

            return inserted > 0;
        }

        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM Genres" +
                " WHERE movieId = @MovieId", new { MovieId = id }));



            var deleted = await connection.ExecuteAsync(new CommandDefinition("DELETE FROM Movies" +
                " WHERE id = @Id", new { Id = id }));

            transaction.Commit();
            return deleted > 0;
        }

        public async Task<bool> ExistsByIdAsync(Guid id)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var exists = await connection.ExecuteScalarAsync<bool>(new CommandDefinition("SELECT count(1) FROM Movies" +
                " WHERE id = @Id", new { Id = id }));
            return exists;
        }

        public async Task<Movie?> GetByIdAsync(Guid id)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("SELECT * FROM Movies" +
                " WHERE id = @Id", new {Id = id}));
            if (movie is null)
            {
                return null;
            }
            var genres = await connection.QueryAsync<string>(new CommandDefinition("SELECT name FROM Genres" +
                " WHERE movieId = @MovieId", new { MovieId = id }));
            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }
            return movie;
        }

        public async Task<Movie?> GetBySlugAsync(string slug)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("SELECT * FROM Movies" +
                " WHERE slug = @Slug", new { Slug = slug }));
            if (movie is null)
            {
                return null;
            }
            var genres = await connection.QueryAsync<string>(new CommandDefinition("SELECT name FROM Genres" +
                " WHERE movieId = @MovieId", new { MovieId = movie.Id }));
            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync()
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var results = await connection.QueryAsync(new CommandDefinition("SELECT mo.*, string_agg(ge.name, ',') as genres" +
                " FROM Movies mo" +
                " LEFT JOIN Genres ge ON mo.id = ge.movieid" +
                " GROUP BY mo.id"));
            var movies = results.Select(res => new Movie
            {
                Id = res.id,
                Title = res.title,
                YearOfRelease = res.yearofrelease,
                Genres = Enumerable.ToList(res.genres.Split(','))
            });
            return movies;
        }

        public async Task<bool> UpdateAsync(Movie movie)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM Genres" +
                " WHERE movieId = @MovieId", new { MovieId = movie.Id }));

            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("INSERT INTO Genres (movieId, name)" +
                    " values (@MovieId, @Name)", new { MovieId = movie.Id, Name = genre }));
            }

            var updated = await connection.ExecuteAsync(new CommandDefinition("UPDATE Movies SET" +
                " id = @Id," +
                " slug = @Slug," +
                " title = @Title," +
                " yearofrelease = @YearOfRelease", movie));

            transaction.Commit();
            return updated > 0;
        }
    }
}
