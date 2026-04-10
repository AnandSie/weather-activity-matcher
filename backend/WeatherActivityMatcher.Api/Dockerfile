# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["WeatherActivityMatcher.Api/WeatherActivityMatcher.Api.csproj", "WeatherActivityMatcher.Api/"]
COPY ["WeatherActivityMatcher.ServiceDefaults/WeatherActivityMatcher.ServiceDefaults.csproj", "WeatherActivityMatcher.ServiceDefaults/"]
RUN dotnet restore "WeatherActivityMatcher.Api/WeatherActivityMatcher.Api.csproj"

COPY . .
RUN dotnet publish "WeatherActivityMatcher.Api/WeatherActivityMatcher.Api.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "WeatherActivityMatcher.Api.dll"]
