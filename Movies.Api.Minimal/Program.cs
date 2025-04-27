using Movies.Api.Minimal.Endpoints;
using Movies.Application;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var connectionString = config["MoviesDb:connectionString"];

// Add services to the container.
builder.Services.AddMoviesApplication();
builder.Services.AddMoviesDatabase(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.MapApiEndpoints();

app.Run();
