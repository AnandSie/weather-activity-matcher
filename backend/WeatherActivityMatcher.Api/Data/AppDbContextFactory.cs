using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WeatherActivityMatcher.Api.Data;

/// <summary>
/// Used by EF Core tooling (dotnet ef migrations add) at design time.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=weather_matcher.db")
            .Options;
        return new AppDbContext(options);
    }
}
