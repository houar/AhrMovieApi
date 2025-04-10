using Dapper;
using Movies.Application.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public RatingRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
        }

        public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
        {
            var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var command = new CommandDefinition("DELETE FROM Ratings" +
                " WHERE movieid = @MovieId AND userid = @UserId", new { MovieId = movieId, UserId = userId }, cancellationToken: token);
            var result = await connection.ExecuteAsync(command);
            return result > 0;
        }

        public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
        {
            var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var command = new CommandDefinition("SELECT ROUND(AVG(rg.rating), 1) AS rating" +
                " FROM Ratings rg" +
                " WHERE rg.movieid = @MovieId", new { MovieId = movieId }, cancellationToken: token);
            var rating = await connection.QuerySingleOrDefaultAsync<float?>(command);
            return rating;
        }

        public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
        {
            var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var command = new CommandDefinition("SELECT ROUND(AVG(rg.rating), 1) AS rating, usr.rating as userrating" +
                " FROM Ratings rg" +
                " LEFT JOIN Ratings usr ON usr.movieid = rg.movieid AND usr.userid = @UserId" +
                " WHERE rg.movieid = @MovieId" +
                " GROUP BY userrating", new { MovieId = movieId, UserId = userId }, cancellationToken: token);
            return await connection.QuerySingleOrDefaultAsync<(float? Rating, int? UserRating)>(command);
        }

        public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
        {
            var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var command = new CommandDefinition("INSERT INTO Ratings (movieid, userid, rating)" +
                " VALUES (@MovieId, @UserId, @Rating)" +
                " ON CONFLICT (movieid, userid) DO UPDATE SET rating = @Rating", new { MovieId = movieId, UserId = userId, Rating = rating }, cancellationToken: token);
            var result = await connection.ExecuteAsync(command);
            return result > 0;
        }
    }
}
