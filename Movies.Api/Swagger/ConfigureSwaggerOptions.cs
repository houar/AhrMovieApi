using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Movies.Api.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IHostEnvironment _env;
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            _env = env;
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = _env.EnvironmentName,
                        Version = description.ApiVersion.ToString()
                    });
            }
        }
    }
}
