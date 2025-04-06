using Dapper;

namespace Movies.Application.Database
{
    public class NpgsqlInitializer
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public NpgsqlInitializer(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task InitializeAsync()
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            await connection.ExecuteAsync("CREATE TABLE IF NOT EXISTS Movies (" +
                " Id UUID PRIMARY KEY," +
                " Title TEXT NOT NULL," +
                " Slug TEXT NOT NULL," +
                " YearOfRelease INTEGER NOT NULL);");
            await connection.ExecuteAsync("CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS movies_slug_idx " +
                "ON Movies " +
                "USING BTREE(Slug)");
            await connection.ExecuteAsync("CREATE TABLE IF NOT EXISTS Genres (" +
                " movieId UUID references movies (Id)," +
                " name TEXT NOT NULL);");
        }
    }
}
