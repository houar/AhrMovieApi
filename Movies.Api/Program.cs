using Movies.Application;
using Movies.Application.Database;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["MoviesDb:connectionString"];

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMoviesApplication();
builder.Services.AddMoviesDatabase(connectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<NpgsqlInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
