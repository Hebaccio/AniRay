using AniRay.Model.Data;
using AniRay.Model.Requests.AuthRequests;
using AniRay.Services.Interfaces;
using AniRay.Services.Services;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Security.Claims;
using System.Text;
using Microsoft.OpenApi.Models;
using AniRay.Services.Services.BaseServices;
using AniRay.Services.Interfaces.BasicServices;
using AniRay.Services.Services.BasicServices;
using AniRay.Services.Services.AuthentificationServices;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IAudioFormatService, AudioFormatService>();
//builder.Services.AddTransient<IGenderService, GenderService>();
//builder.Services.AddTransient<IGenreService, GenreService>();
//builder.Services.AddTransient<IOrderStatusService, OrderStatusService>();
//builder.Services.AddTransient<IUserRoleService, UserRoleService>();
//builder.Services.AddTransient<IUserStatusService, UserStatusService>();
//builder.Services.AddTransient<IVideoFormatService, VideoFormatService>();
//builder.Services.AddScoped<IBluRayService, BluRayService>();
//builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();



// Controllers
builder.Services.AddControllers()
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
