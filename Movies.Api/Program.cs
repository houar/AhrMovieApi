using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application;
using Movies.Application.Database;

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
        ctx.User.HasClaim(claim => claim is { Type: AuthConstants.TrustedMemberClaimName, Value: AuthConstants.TrustedMemberClaimValue}));
    });
});
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1.0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    //options.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
    options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
}).AddMvc();
builder.Services.AddControllers();
builder.Services.AddMoviesApplication();
builder.Services.AddMoviesDatabase(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<NpgsqlInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
