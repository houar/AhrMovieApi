using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Movies.Api.Sdk.Consumer.MovieApi
{
    internal class AuthTokenProvider
    {
        private readonly HttpClient _httpClient;
        private static readonly SemaphoreSlim _lock = new(1, 1);
        private AuthToken _authToken;
        private MovieApiOptions _options;

        public AuthTokenProvider(HttpClient httpClient, IOptions<MovieApiOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<string> GetAuthToken()
        {
            try
            {
                if (_authToken == null || _authToken.Expired)
                {
                    await _lock.WaitAsync();
                    var authEndpoint = _options.AuthEndpoint;
                    var response = await _httpClient.PostAsJsonAsync(authEndpoint, new
                    {
                        userid = "d8566de3-b1a6-4a9b-b842-8e3887a82e41",
                        email = "madjid@houar.eu",
                        customClaims = new Dictionary<string, object>
                            {
                                {"admin", true },
                                {"trusted_member", true}
                            }
                    });
                    response.EnsureSuccessStatusCode();
                    var token = await response.Content.ReadAsStringAsync();
                    _authToken = new AuthToken(token);
                }
            }
            finally
            {
                _lock.Release();
            }
            return _authToken.BearerToken;
        }
    }
}
