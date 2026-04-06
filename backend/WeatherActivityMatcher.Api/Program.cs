using Microsoft.EntityFrameworkCore;
using WeatherActivityMatcher.Api.BackgroundServices;
using WeatherActivityMatcher.Api.Data;
using WeatherActivityMatcher.Api.Services;
using WeatherActivityMatcher.Api.Services.Notifications;
// ServiceDefaults extension methods are in Microsoft.Extensions.Hosting namespace (Aspire pattern)

var builder = WebApplication.CreateBuilder(args);

// Aspire service defaults (health checks, OpenTelemetry, resilience)
builder.AddServiceDefaults();

// Database
var dbPath = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=weather_matcher.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(dbPath));

// HTTP client for Open-Meteo (no API key needed)
builder.Services.AddHttpClient("openmeteo", client =>
{
    client.BaseAddress = new Uri("https://api.open-meteo.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Application services — Singletons that manage their own scopes via IServiceScopeFactory
builder.Services.AddSingleton<WeatherService>();
builder.Services.AddSingleton<MatcherService>();

// Notification services
builder.Services.AddSingleton<INotificationService, InAppNotificationService>();
builder.Services.AddSingleton<NotificationRegistry>();

// Background services
builder.Services.AddHostedService<WeatherRefreshBackgroundService>();
builder.Services.AddHostedService<MatchingBackgroundService>();

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Weather Activity Matcher API", Version = "v1" });
});

// CORS for local development (Vite dev server)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:80", "http://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapDefaultEndpoints(); // /health and /alive from ServiceDefaults

app.Run();
