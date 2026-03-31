using AniRay.API.Filters;
using AniRay.Model.AuthRequests;
using AniRay.Model.Data;
using AniRay.Services.AuthentificationServices.AuthService;
using AniRay.Services.AuthentificationServices.TokenService;
using AniRay.Services.EntityServices.AudioFormatService;
using AniRay.Services.EntityServices.BluRayService;
using AniRay.Services.EntityServices.GenderService;
using AniRay.Services.EntityServices.GenreService;
using AniRay.Services.EntityServices.MovieService;
using AniRay.Services.EntityServices.OrderService;
using AniRay.Services.EntityServices.OrderStatusService;
using AniRay.Services.EntityServices.RequestService;
using AniRay.Services.EntityServices.UserCartService;
using AniRay.Services.EntityServices.UserFavoritesService;
using AniRay.Services.EntityServices.UserRoleService;
using AniRay.Services.EntityServices.UserService;
using AniRay.Services.EntityServices.UserStatusService;
using AniRay.Services.EntityServices.VideoFormatService;
using AniRay.Services.HelperServices.CurrentUserService;
using AniRay.Services.HelperServices.MailService;
using AniRay.Services.HelperServices.NotificationThing;
using AniRay.Services.HelperServices.OtherHelpers;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IAudioFormatService, AudioFormatService>();
builder.Services.AddTransient<IGenderService, GenderService>();
builder.Services.AddTransient<IGenreService, GenreService>();
builder.Services.AddTransient<IOrderStatusService, OrderStatusService>();
builder.Services.AddTransient<IUserRoleService, UserRoleService>();
builder.Services.AddTransient<IUserStatusService, UserStatusService>();
builder.Services.AddTransient<IVideoFormatService, VideoFormatService>();
builder.Services.AddScoped<IBluRayService, BluRayService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserCartService, UserCartService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IUserFavoritesService, UserFavoritesService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddSingleton<BluRayNotificationService>();
//builder.Services.AddHostedService(sp => sp.GetRequiredService<BluRayNotificationService>());

builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
builder.Services.AddHostedService<EmailConsumerService>();

builder.Services.Configure<RabbitMqDetails>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<RabbitMqDetails>>().Value);

MapsterConfig.RegisterMappings();

// Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters
        .Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});


#region Authentication & JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),

            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Workers",
        policy => policy.RequireRole("Employee", "Boss"));
});

#endregion

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AniRay API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer' [space] and then your valid token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});


builder.Services.AddDbContext<AniRayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMapster();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Swagger ONLY in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        options.RoutePrefix = "swagger";
        options.DocExpansion(DocExpansion.None);
    });

    // Redirect / ? /swagger
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
