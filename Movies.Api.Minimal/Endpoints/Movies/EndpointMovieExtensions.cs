namespace Movies.Api.Minimal.Endpoints.Movies
{
    internal static class EndpointMovieExtensions
    {
        internal static void MapMovieEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapCreateMovie();
            app.MapGetMovie();
            app.MapGetAllMovies();
            //app.MapUpdateMovie();
            //app.MapDeleteMovie();
        }
    }
}
