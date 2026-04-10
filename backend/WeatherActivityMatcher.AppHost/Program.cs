var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.WeatherActivityMatcher_Api>("api")
    .WithHttpEndpoint(port: 5000, name: "http");

// Development: run Vite dev server.
// Production (azd deploy): PublishAsDockerFile() switches to the frontend/Dockerfile.
// API_UPSTREAM is injected so the nginx template has the correct upstream URL in both modes.
builder.AddNpmApp("frontend", "../../frontend", scriptName: "dev")
    .WithReference(api)
    .WithEnvironment("VITE_API_BASE_URL", api.GetEndpoint("http"))
    .WithEnvironment("API_UPSTREAM", api.GetEndpoint("http"))
    .WithHttpEndpoint(port: 5173, env: "VITE_PORT")
    .PublishAsDockerFile();

builder.Build().Run();
