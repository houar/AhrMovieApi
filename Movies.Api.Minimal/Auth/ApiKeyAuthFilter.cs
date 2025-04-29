using Movies.Api.Minimal.Auth;

namespace Movies.Api.Auth
{
    public class ApiKeyAuthFilter : IEndpointFilter
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthFilter(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var exists = context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var requestApiKey);

            if (exists == false)
            {
                //return Results.Unauthorized();
                return Results.Problem(
                    title: "Unauthorized",
                    detail: "The API Key is missing.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            var expectedApiKey = _configuration["ApiKey"];

            if (requestApiKey != expectedApiKey)
            {
                //return Results.Unauthorized();
                return Results.Problem(
                    title: "Unauthorized",
                    detail: "The API Key is invalid.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            return await next(context);
        }
    }
}
