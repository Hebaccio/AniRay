using AniRay.Model.Data;
using AniRay.Services.Interfaces;
using AniRay.Services.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IMoviesService, MoviesService>();
builder.Services.AddTransient<IBluRaysService, BluRayService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAudioFormatService, AudioFormatService>();
builder.Services.AddTransient<IGenderService, GenderService>();
builder.Services.AddTransient<IGenreService, GenreService>();
builder.Services.AddTransient<IOrderStatusService, OrderStatusService>();
builder.Services.AddTransient<IUserRoleService, UserRoleService>();
builder.Services.AddTransient<IUserStatusService, UserStatusService>();
builder.Services.AddTransient<IVideoFormatService, VideoFormatService>();


// Controllers
//builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AniRayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMapster();

var app = builder.Build();

// Swagger ONLY in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        options.RoutePrefix = "swagger";
    });

    // Redirect / ? /swagger
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
