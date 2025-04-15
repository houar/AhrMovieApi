using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Movies.Api.Auth;
using Movies.Api.Health;
using Movies.Api.Mapping;
using Movies.Api.Swagger;
using Movies.Application;
using Movies.Application.Database;
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
        //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
});
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1.0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        //options.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
        options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
    })
    .AddMvc()
    .AddApiExplorer();
builder.Services.AddHealthChecks().AddCheck<DBHealthCheck>(DBHealthCheck.HealthCheckName);
builder.Services.AddControllers();
builder.Services.AddMoviesApplication();
builder.Services.AddMoviesDatabase(connectionString);
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
        //x.RoutePrefix = string.Empty;
    });
}

// Configure the HTTP request pipeline.

app.MapHealthChecks("_health");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<NpgsqlInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
