var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.WeatherActivityMatcher_Api>("api")
    .WithHttpEndpoint(port: 5000, name: "http");

// In development: run Vite dev server
builder.AddNpmApp("frontend", "../../frontend", scriptName: "dev")
    .WithReference(api)
    .WithEnvironment("VITE_API_BASE_URL", api.GetEndpoint("http"))
    .WithHttpEndpoint(port: 5173, env: "VITE_PORT")
    .ExcludeFromManifest(); // Dev-only; container mode uses docker-compose

builder.Build().Run();
