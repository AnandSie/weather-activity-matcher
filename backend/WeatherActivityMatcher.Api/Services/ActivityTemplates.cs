using WeatherActivityMatcher.Api.Dtos;

namespace WeatherActivityMatcher.Api.Services;

public static class ActivityTemplates
{
    // DaysOfWeek bitmask: Mon=1, Tue=2, Wed=4, Thu=8, Fri=16, Sat=32, Sun=64
    private const int AllDays = 127;
    private const int WeekendDays = 96; // Sat=32 + Sun=64

    public static readonly IReadOnlyList<TemplateDto> All =
    [
        new("windsurfing", "Windsurfing", new ConstraintsDto(
            TempMin: 15, TempMax: 35,
            WindMin: 20, WindMax: 60,
            CloudMin: null, CloudMax: null,
            PrecipMax: 2.5,
            TimeStart: "08:00", TimeEnd: "20:00",
            DaysOfWeek: AllDays
        )),
        new("kitesurfing", "Kitesurfing", new ConstraintsDto(
            TempMin: 15, TempMax: 35,
            WindMin: 25, WindMax: 70,
            CloudMin: null, CloudMax: null,
            PrecipMax: 2.5,
            TimeStart: "08:00", TimeEnd: "20:00",
            DaysOfWeek: AllDays
        )),
        new("bbq", "BBQ", new ConstraintsDto(
            TempMin: 18, TempMax: 35,
            WindMin: null, WindMax: 20,
            CloudMin: null, CloudMax: 60,
            PrecipMax: 0,
            TimeStart: "11:00", TimeEnd: "22:00",
            DaysOfWeek: AllDays
        )),
        new("soccer", "Soccer", new ConstraintsDto(
            TempMin: 5, TempMax: 30,
            WindMin: null, WindMax: 30,
            CloudMin: null, CloudMax: null,
            PrecipMax: 2.5,
            TimeStart: "08:00", TimeEnd: "21:00",
            DaysOfWeek: AllDays
        )),
        new("cycling", "Cycling", new ConstraintsDto(
            TempMin: 8, TempMax: 30,
            WindMin: null, WindMax: 35,
            CloudMin: null, CloudMax: 80,
            PrecipMax: 0,
            TimeStart: "07:00", TimeEnd: "20:00",
            DaysOfWeek: AllDays
        )),
        new("beach", "Beach Day", new ConstraintsDto(
            TempMin: 22, TempMax: 40,
            WindMin: null, WindMax: 25,
            CloudMin: null, CloudMax: 40,
            PrecipMax: 0,
            TimeStart: "10:00", TimeEnd: "19:00",
            DaysOfWeek: WeekendDays
        )),
    ];

    public static TemplateDto? FindBySlug(string slug) =>
        All.FirstOrDefault(t => t.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
}
