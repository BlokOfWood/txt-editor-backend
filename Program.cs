using aresu_txt_editor_backend.Data;
using aresu_txt_editor_backend.Interfaces;
using aresu_txt_editor_backend.Models;
using aresu_txt_editor_backend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add auth configuration from appsettings.json
var configSection = builder.Configuration.GetSection("Auth");

if (!configSection.Exists())
{
    throw new Exception("Auth configuration section is missing in appsettings.json");
}

builder.Services.Configure<AuthConfiguration>(configSection);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<MssqlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddWebSockets((options) =>
{
    options.KeepAliveTimeout = TimeSpan.FromSeconds(5);
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
});

var authConfig = configSection.Get<AuthConfiguration>() ?? throw new Exception("Failed to bind Auth configuration");

// Cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "AuthToken";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.MaxAge = TimeSpan.FromDays(authConfig.AccessTokenExpirationDays);
        options.ExpireTimeSpan = TimeSpan.FromDays(authConfig.AccessTokenExpirationDays);

        options.SlidingExpiration = true;

        options.LoginPath = "/user/login";
        options.LogoutPath = "/user/logout";

        // Return 401 instead of redirecting to login page for API
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddSingleton<IOccupancyService, OccupancyService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(policy =>
    policy.WithOrigins("http://localhost:4200")
            .AllowCredentials()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();

app.MapControllers();

app.Run("http://localhost:5129");