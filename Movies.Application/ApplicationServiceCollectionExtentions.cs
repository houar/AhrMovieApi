using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repositories;

namespace Movies.Application
{
    public static class ApplicationServiceCollectionExtentions
    {
        public static void AddMoviesApplication(this IServiceCollection services)
        {
            services.AddSingleton<IMovieRepository, MovieRepository>();
        }

        public static void AddMoviesDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
            services.AddSingleton<NpgsqlInitializer>();
        }
    }
}
