
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer.MovieApi;
using Movies.Contracts.Requests.V1;
using Refit;
using System.Net.Http.Headers;
using System.Text.Json;

/*
* Read the config
*/

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
var config = builder.Build();
var apiUrl = config.GetValue<string>("MoviesApiOptions:BaseUrl");

/*
* Include the API version in : Query
*/

var queryVersion = new QueryParams() { apiversion = "2.0" };

var movieApiWithQueryVersion = RestService.For<IMoviesApi>(apiUrl);

var movieQueryVersion = await movieApiWithQueryVersion.GetMovieApiVersionQueryAsync(queryVersion, "nick-the-greek-2023");

string jsonQueryVersion = JsonSerializer.Serialize(movieQueryVersion, new JsonSerializerOptions { WriteIndented = true });

var linesQueryVersion = new[] { "\nGet movie: Version infos -> Query", "----------", jsonQueryVersion };
Console.WriteLine(string.Join(Environment.NewLine, linesQueryVersion));

/*
* Include the API version in : Custom header (x-api-version)
* x-api-version = "2.0"
*/

var httpClientWithCustomHeader = new HttpClient()
{
    BaseAddress = new Uri(apiUrl)
};

httpClientWithCustomHeader.DefaultRequestHeaders.Add("x-api-version", config.GetValue<string>("MoviesApiOptions:ApiVersion"));

var movieApiWithCustomHeader = RestService.For<IMoviesApi>(httpClientWithCustomHeader);

var movieCustomHeader = await movieApiWithCustomHeader.GetMovieAsync("nick-the-greek-2023");

string jsonCustomHeader = JsonSerializer.Serialize(movieCustomHeader, new JsonSerializerOptions { WriteIndented = true });

var linesCustomHeaderReponse = new[] { "\nGet movie: Version infos -> Custom header", "----------", jsonCustomHeader };
Console.WriteLine(string.Join(Environment.NewLine, linesCustomHeaderReponse));

/*
* Include the API version in : Default custom header (x-api-version)
* x-api-version = "2.0"
*/

var movieApiWithDefaultCustomHeader = RestService.For<IMoviesApi>(apiUrl);

var movieDefaultCustomHeader = await movieApiWithDefaultCustomHeader.GetMovieApiVersionHeaderAsync("nick-the-greek-2023");

string jsonDefaultCustomHeader = JsonSerializer.Serialize(movieDefaultCustomHeader, new JsonSerializerOptions { WriteIndented = true });

var linesDefaultCustomHeaderReponse = new[] { "\nGet movie: Version infos -> Default custom header", "----------", jsonDefaultCustomHeader };
Console.WriteLine(string.Join(Environment.NewLine, linesDefaultCustomHeaderReponse));

/*
* Include the API version in : MediaType (Accept header)
* Accept = "application/json; x-api-version=2.0"
*/

var handlerNoContent = new HttpClientHandler();

var httpClientWithAcceptHeader = new HttpClient(handlerNoContent)
{
    BaseAddress = new Uri(apiUrl)
};

var mediaType = new MediaTypeWithQualityHeaderValue("application/json");
mediaType.Parameters.Add(new NameValueHeaderValue("x-api-version", "2.0"));
httpClientWithAcceptHeader.DefaultRequestHeaders.Accept.Add(mediaType);

var movieApiWithAcceptHeader = RestService.For<IMoviesApi>(httpClientWithAcceptHeader);

var movieWithAcceptHeader = await movieApiWithAcceptHeader.GetMovieAsync("nick-the-greek-2023");

string jsonWithAcceptHeader = JsonSerializer.Serialize(movieWithAcceptHeader, new JsonSerializerOptions { WriteIndented = true });

var linesWithAcceptHeader = new[] { "\nGet movie: Version infos -> Accept header", "----------", jsonWithAcceptHeader };
Console.WriteLine(string.Join(Environment.NewLine, linesWithAcceptHeader));

/*
* Include the API version in : ContentType header
* ContentType = "application/json; x-api-version=2.0"
*/

var handlerWithContent = new MovieApiPostHandler
{
    InnerHandler = new HttpClientHandler()
};

var httpClientForPost = new HttpClient(handlerWithContent)
{
    BaseAddress = new Uri(apiUrl)
};

var movieApiForPost = RestService.For<IMoviesApi>(httpClientForPost);

var updateMovieResponse = await movieApiForPost.UpdateMovie(Guid.Parse("26e3b929-9049-4a4b-8314-a1fe589698b9"), new MovieReqUpdate
{
    Title = "Nick the Greek 2",
    YearOfRelease = 2025,
    Genres = [
        "Comedy",
        "Fiction",
        "Fiction"
    ]
});

string jsonUpdate = JsonSerializer.Serialize(updateMovieResponse.Content, new JsonSerializerOptions { WriteIndented = true });

var update = new[] { "\nUpdate movie: Version infos -> ContentType header", "-------------", jsonUpdate };
Console.WriteLine(string.Join(Environment.NewLine, update));

/*
* 08 - 05/09 : Using the HttpClientFactory
*/

var services = new ServiceCollection();
services.AddRefitClient<IMoviesApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl));
var serviceProvider = services.BuildServiceProvider();
var movieApiWithHttpClientFactory = serviceProvider.GetRequiredService<IMoviesApi>();
var movieWithHttpClientFactory = await movieApiWithHttpClientFactory.GetMovieApiVersionHeaderAsync("nick-the-greek-2023");
string jsonWithHttpClientFactory = JsonSerializer.Serialize(movieWithHttpClientFactory, new JsonSerializerOptions { WriteIndented = true });
var printWithHttpClientFactory = new[] {
    "\n08 - 05/09 > Using the HttpClientFactory:",
    "-----------------------------------------",
    jsonWithHttpClientFactory };
Console.WriteLine(string.Join(Environment.NewLine, printWithHttpClientFactory));

/*
* 08 - 06/09 : Adding authentication
* 08 - 07/09 : Handling token generation and refreshing
*/

services
    .AddHttpClient()
    .Configure<MovieApiOptions>(config.GetSection("MoviesApiOptions"))
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApiV1Auth>(provider => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async (message, token) =>
        {
            var tokenProvider = provider.GetRequiredService<AuthTokenProvider>();
            return await tokenProvider.GetAuthToken();
        }
    })
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl));
var serviceProvider2 = services.BuildServiceProvider();
var movieApiV1NeedsAuth = serviceProvider2.GetRequiredService<IMoviesApiV1Auth>();
var movieV1NeedsAuth = await movieApiV1NeedsAuth.GetMovieV1NeedsAuthorizationAsync("nick-the-greek-2023");
string jsonV1NeedsAuth = JsonSerializer.Serialize(movieV1NeedsAuth, new JsonSerializerOptions { WriteIndented = true });
var printV1NeedsAuth = new[] {
    "\n08 - 06/09 > Adding authentication:",
    "-----------------------------------",
    "\n08 - 07/09 > Handling token generation and refreshing:",
    "------------------------------------------------------",
    jsonV1NeedsAuth };
Console.WriteLine(string.Join(Environment.NewLine, printV1NeedsAuth));

Console.ReadKey();
