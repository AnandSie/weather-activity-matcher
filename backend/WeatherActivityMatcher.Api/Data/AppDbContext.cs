using Microsoft.EntityFrameworkCore;
using WeatherActivityMatcher.Api.Models;

namespace WeatherActivityMatcher.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<WeatherConstraints> WeatherConstraints => Set<WeatherConstraints>();
    public DbSet<HourlyForecast> HourlyForecasts => Set<HourlyForecast>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherConstraints>()
            .HasOne(wc => wc.Activity)
            .WithOne(a => a.Constraints)
            .HasForeignKey<WeatherConstraints>(wc => wc.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HourlyForecast>()
            .HasIndex(f => f.ValidTime)
            .IsUnique();

        // Seed default user settings (singleton row)
        modelBuilder.Entity<UserSettings>().HasData(
            new UserSettings { Id = 1, Latitude = 52.37, Longitude = 4.90, LocationName = "Amsterdam" }
        );
    }
}
