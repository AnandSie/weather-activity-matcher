namespace WeatherActivityMatcher.Api.Dtos;

public record ConstraintsDto(
    double? TempMin,
    double? TempMax,
    double? WindMin,
    double? WindMax,
    double? CloudMin,
    double? CloudMax,
    double? PrecipMax,
    string? TimeStart,
    string? TimeEnd,
    int? DaysOfWeek
);

public record ActivityCreateDto(
    string Name,
    string? TemplateSlug,
    bool Enabled,
    ConstraintsDto Constraints
);

public record ActivityUpdateDto(
    string Name,
    bool Enabled,
    ConstraintsDto Constraints
);

public record ActivityDto(
    int Id,
    string Name,
    string? TemplateSlug,
    bool Enabled,
    DateTime CreatedAt,
    ConstraintsDto? Constraints
);

public record TemplateDto(
    string Slug,
    string Name,
    ConstraintsDto Constraints
);
