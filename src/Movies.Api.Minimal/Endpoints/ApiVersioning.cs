using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;

namespace Movies.Api.Minimal.Endpoints
{
    internal static class ApiVersioning
    {
        internal static ApiVersionSet VersionSet { get; private set; }

        internal static void CreateApiVersionSet(this IEndpointRouteBuilder app)
        {
            VersionSet = app.NewApiVersionSet()
                .HasApiVersion(1.0)
                .HasApiVersion(2.0)
                .HasApiVersion(3.0)
                .ReportApiVersions()
                .Build();
        }
    }
}
