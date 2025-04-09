using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MovieRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
        }

        public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction();
            var inserted = await connection.ExecuteAsync(new CommandDefinition("INSERT INTO Movies" +
                " (id, slug, title, yearofrelease)" +
                " values (@Id, @Slug, @Title, @YearOfRelease)", movie, cancellationToken: token));
            if (inserted > 0)
            {
                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(new CommandDefinition("INSERT INTO Genres" +
                        " (movieId, name)" +
                        " values (@MovieId, @Name)", new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
                }
            }
            transaction.Commit();

            return inserted > 0;
        }

        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM Genres" +
                " WHERE movieId = @MovieId", new { MovieId = id }, cancellationToken: token));



            var deleted = await connection.ExecuteAsync(new CommandDefinition("DELETE FROM Movies" +
                " WHERE id = @Id", new { Id = id }, cancellationToken: token));

            transaction.Commit();
            return deleted > 0;
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var exists = await connection.ExecuteScalarAsync<bool>(new CommandDefinition("SELECT count(1) FROM Movies" +
                " WHERE id = @Id", new { Id = id }, cancellationToken: token));
            return exists;
        }

        public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("SELECT * FROM Movies" +
                " WHERE id = @Id", new {Id = id}, cancellationToken: token));
            if (movie is null)
            {
                return null;
            }
            var genres = await connection.QueryAsync<string>(new CommandDefinition("SELECT name FROM Genres" +
                " WHERE movieId = @MovieId", new { MovieId = id }, cancellationToken: token));
            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }
            return movie;
        }

        public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("SELECT * FROM Movies" +
                " WHERE slug = @Slug", new { Slug = slug }, cancellationToken: token));
            if (movie is null)
            {
                return null;
            }
            var genres = await connection.QueryAsync<string>(new CommandDefinition("SELECT name FROM Genres" +
                " WHERE movieId = @MovieId", new { MovieId = movie.Id }, cancellationToken: token));
            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync(Guid? userId = default, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var results = await connection.QueryAsync(new CommandDefinition("SELECT mo.*, string_agg(ge.name, ',') as genres" +
                " FROM Movies mo" +
                " LEFT JOIN Genres ge ON mo.id = ge.movieid" +
                " GROUP BY mo.id", cancellationToken: token));
            var movies = results.Select(res => new Movie
            {
                Id = res.id,
                Title = res.title,
                YearOfRelease = res.yearofrelease,
                Genres = Enumerable.ToList(res.genres.Split(','))
            });
            return movies;
        }

        public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM Genres" +
                " WHERE movieId = @MovieId", new { MovieId = movie.Id }, cancellationToken: token));

            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("INSERT INTO Genres (movieId, name)" +
                    " values (@MovieId, @Name)", new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
            }

            var updated = await connection.ExecuteAsync(new CommandDefinition("UPDATE Movies SET" +
                " slug = @Slug," +
                " title = @Title," +
                " yearofrelease = @YearOfRelease" +
                " WHERE id = @Id", movie, cancellationToken: token));

            transaction.Commit();
            return updated > 0;
        }
    }
}
