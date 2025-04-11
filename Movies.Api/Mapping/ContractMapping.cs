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

        public static MoviesResponse MapToMoviesResponse(this IEnumerable<Movie> movies, int page, int pageSize, int total)
        {
            return new MoviesResponse
            {
                Items = movies.Select(MapToMovieResponse),
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public static RatingResponse MapToRatingResponse(this Rating rating)
        {
            return new RatingResponse
            {
                MovieId = rating.MovieId,
                MovieSlug = rating.MovieSlug,
                MovieTitle = rating.MovieTitle,
                UserRating = rating.UserRating
            };
        }

        public static RatingsResponse MapToRatingsResponse(this IEnumerable<Rating> ratings)
        {
            return new RatingsResponse
            {
                Ratings = ratings.Select(MapToRatingResponse)
            };
        }

        public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest request)
        {
            return new GetAllMoviesOptions
            {
                Title = request.Title,
                YearOfRelease = request.Year,
                SortField = request.SortBy?.Trim('+','-'),
                SortOrder = request.SortBy is null ? SortOrder.Unsorted : request.SortBy?.StartsWith('-') == true ? SortOrder.Descending : SortOrder.Ascending,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid? userId)
        {
            options.UserId = userId;
            return options;
        }
    }
}
