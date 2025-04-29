using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Movies.Api.Auth;
using Movies.Api.Minimal.Auth;
using Movies.Api.Minimal.Endpoints;
using Movies.Api.Minimal.Mapping;
using Movies.Api.Minimal.OutputCache;
using Movies.Api.Minimal.Swagger;
using Movies.Application;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var connectionString = config["MoviesDb:connectionString"];
var signKey = config["JwtValidation:SigningKey"];
var iss = config["JwtValidation:Issuer"];
var aud = config["JwtValidation:Audience"];

// Add services to the container.
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = iss,
            ValidAudience = aud
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthConstants.AdminUserPolicyName, policy =>
    {
        //policy.RequireAuthenticatedUser();
        policy.RequireClaim(AuthConstants.AdminUserClaimName, AuthConstants.AdminUserClaimValue);
    });
    options.AddPolicy(AuthConstants.AdminOrTrustedPolicyName, policy =>
    {
        policy.RequireAssertion(ctx =>
        ctx.User.HasClaim(claim => claim is { Type: AuthConstants.AdminUserClaimName, Value: AuthConstants.AdminUserClaimValue }) ||
        ctx.User.HasClaim(claim => claim is { Type: AuthConstants.TrustedMemberClaimName, Value: AuthConstants.TrustedMemberClaimValue }));
    });
    options.AddPolicy(AuthConstants.MultiAuthPolicyName, policy =>
    {
        policy.Requirements.Add(new MultiAuthRequirement(config["ApiKey"]));
    });
});
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1.0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version"));
})
.AddApiExplorer();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMoviesApplication();
builder.Services.AddMoviesDatabase(connectionString);
builder.Services.AddResponseCaching();
builder.Services.AddOutputCache(op =>
{
    op.AddBasePolicy(x => x.Cache());
    op.AddPolicy("MovieGetAll", c =>
    {
        c.Cache()
        .Expire(TimeSpan.FromMinutes(1))
        .SetVaryByQuery(new[] { "title", "year", "sortBy", "page", "pageSize" })
        .Tag("movie-get-all");
    });
    op.AddPolicy("MovieGetWithUserRat", new ClaimAwareCachePolicy("userid"));
});
builder.Services.AddScoped<IEndpointFilter, ApiKeyAuthFilter>();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();
app.UseOutputCache();
app.UseMiddleware<ValidationMappingMiddleware>();
app.MapApiEndpoints();

app.Run();
