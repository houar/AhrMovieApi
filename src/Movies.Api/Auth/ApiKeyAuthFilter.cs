using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Movies.Api.Auth
{
    public class ApiKeyAuthFilter : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthFilter(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var exists = context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var requestApiKey);

            if (exists == false)
            {
                context.Result = new UnauthorizedObjectResult("The API Key is missing");
            }

            var expectedApiKey = _configuration["ApiKey"];

            if (requestApiKey != expectedApiKey)
            {
                context.Result = new UnauthorizedObjectResult("The API Key is invalid");
            }

            return Task.CompletedTask;
        }
    }
}
