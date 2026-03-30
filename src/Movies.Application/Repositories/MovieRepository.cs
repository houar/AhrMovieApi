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
            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("SELECT mo.*," +
                " ROUND(AVG(rg.rating), 1) AS rating, usr.rating as userrating" +
                " FROM Movies mo" +
                " LEFT JOIN Ratings rg ON rg.movieid = mo.id" +
                " LEFT JOIN Ratings usr ON usr.movieid = mo.id AND usr.userid = @UserId" +
                " WHERE mo.id = @Id" +
                " GROUP BY mo.id, userrating", new { Id = id, UserId = userId }, cancellationToken: token));
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
            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("SELECT mo.*," +
                " ROUND(AVG(rg.rating), 1) AS rating, usr.rating as userrating" +
                " FROM Movies mo" +
                " LEFT JOIN Ratings rg ON rg.movieid = mo.id" +
                " LEFT JOIN Ratings usr ON usr.movieid = mo.id AND usr.userid = @UserId" +
                " WHERE mo.slug = @Slug" +
                " GROUP BY mo.id, userrating", new { Slug = slug, UserId = userId }, cancellationToken: token));
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

        public async Task<IEnumerable<Movie>> GetMoviesAsync(GetAllMoviesOptions options, CancellationToken token = default)
        {
            var sortClause = string.Empty;
            if (options.SortOrder != SortOrder.Unsorted)
            {
                var sortField = options.SortField!.Equals("year") ? "yearofrelease" : options.SortField;

                sortClause = options.SortOrder == SortOrder.Ascending
                    ? $", mo.{sortField} ORDER BY {sortField} ASC"
                    : $", mo.{sortField} ORDER BY {sortField} DESC";
            }

            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var results = await connection.QueryAsync(new CommandDefinition("SELECT mo.*, string_agg(distinct ge.name, ',') as genres," +
                " ROUND(AVG(rg.rating), 1) AS rating, usr.rating as userrating" +
                " FROM Movies mo" +
                " LEFT JOIN Genres ge ON mo.id = ge.movieid" +
                " LEFT JOIN Ratings rg ON rg.movieid = mo.id" +
                " LEFT JOIN Ratings usr ON usr.movieid = mo.id AND usr.userid = @UserId" +
                " WHERE (@Title IS NULL OR mo.title ILIKE '%' || @Title || '%')" +
                " AND (@YearOfRelease IS NULL OR mo.yearofrelease = @YearOfRelease)" +
                $" GROUP BY mo.id, userrating {sortClause}" +
                " LIMIT @PageSize" +
                " OFFSET (@Page - 1) * @PageSize", options, cancellationToken: token));
            var movies = results.Select(res => new Movie
            {
                Id = res.id,
                Title = res.title,
                Rating = (float?)res.rating,
                UserRating = (int?)res.userrating,
                YearOfRelease = res.yearofrelease,
                Genres = Enumerable.ToList(res.genres.Split(','))
            });
            return movies;
        }

        public async Task<int> GetCountAsync(string? title, int? year, CancellationToken token = default)
        {
            using var connection = _dbConnectionFactory.CreateConnectionAsync(token).Result;
            var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition("SELECT count(1) FROM Movies" +
                " WHERE (@Title IS NULL OR title ILIKE '%' || @Title || '%')" +
                " AND (@YearOfRelease IS NULL OR yearofrelease = @YearOfRelease)", new { Title = title, YearOfRelease = year }, cancellationToken: token));
            return count;
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
