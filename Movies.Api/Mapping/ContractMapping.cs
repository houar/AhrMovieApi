using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping
{
    public static class ContractMapping
    {
        public static Movie MapToMovie(this MovieReqCreate movieReq)
        {
            return new Movie
            {
                Id = Guid.NewGuid(),
                Title = movieReq.Title,
                YearOfRelease = movieReq.YearOfRelease,
                Genres = movieReq.Genres.ToList()
            };
        }

        public static Movie MapToMovie(this MovieReqUpdate movieReq, Guid id)
        {
            return new Movie
            {
                Id = id,
                Title = movieReq.Title,
                YearOfRelease = movieReq.YearOfRelease,
                Genres = movieReq.Genres.ToList()
            };
        }

        public static MovieResponse MapToMovieResponse(this Movie movie)
        {
            return new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                Slug = movie.Slug,
                Rating = movie.Rating,
                UserRating = movie.UserRating,
                YearOfRelease = movie.YearOfRelease,
                Genres = movie.Genres.ToList()
            };
        }

        public static MoviesResponse MapToMoviesResponse(this IEnumerable<Movie> movies)
        {
            return new MoviesResponse
            {
                Movies = movies.Select(MapToMovieResponse)
            };
        }
    }
}
