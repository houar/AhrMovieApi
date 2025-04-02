using Movies.Application.Models;
using Movies.Contracts.Requests;

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
    }
}
