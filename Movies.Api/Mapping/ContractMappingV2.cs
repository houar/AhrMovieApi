using Movies.Application.Models;
using Movies.Contracts.Requests.V2;
using Movies.Contracts.Responses.V2;

namespace Movies.Api.Mapping
{
    public static class ContractMappingV2
    {
        public static MovieResponseV2 MapToMovieResponseV2(this Movie movie)
        {
            return new MovieResponseV2
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

        public static MoviesResponseV2 MapToMoviesResponseV2(this IEnumerable<Movie> movies, int page, int pageSize, int total)
        {
            return new MoviesResponseV2
            {
                Items = movies.Select(MapToMovieResponseV2),
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequestV2 request)
        {
            return new GetAllMoviesOptions
            {
                Title = request.Title,
                YearOfRelease = request.Year,
                SortField = request.SortBy?.Trim('+', '-'),
                SortOrder = request.SortBy is null ? SortOrder.Unsorted : request.SortBy?.StartsWith('-') == true ? SortOrder.Descending : SortOrder.Ascending,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public static GetAllMoviesOptions WithUserV2(this GetAllMoviesOptions options, Guid? userId)
        {
            options.UserId = userId;
            return options;
        }
    }
}
