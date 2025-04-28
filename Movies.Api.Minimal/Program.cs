using Microsoft.AspNetCore.Authentication.JwtBearer;
using Movies.Api.Minimal.Endpoints;
using Movies.Api.Minimal.Mapping;
using Movies.Api.Minimal.OutputCache;
using Movies.Application;

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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseResponseCaching();
app.UseOutputCache();
app.UseMiddleware<ValidationMappingMiddleware>();
app.MapApiEndpoints();

app.Run();
