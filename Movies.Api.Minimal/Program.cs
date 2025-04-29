using Microsoft.AspNetCore.Authentication.JwtBearer;
using Movies.Api.Auth;
using Movies.Api.Minimal.Auth;
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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();
app.UseOutputCache();
app.UseMiddleware<ValidationMappingMiddleware>();
app.MapApiEndpoints();

app.Run();
