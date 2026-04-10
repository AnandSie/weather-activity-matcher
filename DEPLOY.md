# Deployment Guide

Two options, ordered from simplest to most Aspire-native.

---

## Option A — Azure Container Apps via `azd` (recommended, Aspire-native)

The AppHost already describes both services. `azd` reads it and provisions
everything on Azure: a Container Registry, a Container Apps environment,
managed HTTPS endpoints, and injects all environment variables automatically.

### Prerequisites

```bash
# Azure Developer CLI
curl -fsSL https://aka.ms/install-azd.sh | bash

# Azure CLI (for login)
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Login
az login
azd auth login
```

An Azure subscription is required (the free tier covers this MVP: Container Apps
has a generous free grant — ~180,000 vCPU-seconds/month).

### First deployment

```bash
cd backend/WeatherActivityMatcher.AppHost

# Initialise azd — detects Aspire, creates azure.yaml + infra/ scaffolding
azd init

# Provision Azure resources + build & push images + deploy (all in one)
azd up
```

`azd up` will ask for:
- **Environment name** (e.g. `weather-matcher-dev`)
- **Azure subscription**
- **Region** (e.g. `westeurope` for Amsterdam proximity)

When it finishes it prints the live URLs, e.g.:
```
frontend  https://frontend.happybeach-abc123.westeurope.azurecontainerapps.io
api       https://api.happybeach-abc123.westeurope.azurecontainerapps.io
```

### Set CORS on the API (one time, after first deploy)

Once you know the frontend URL, give it to the API:

```bash
azd env set CORS_ORIGINS https://frontend.happybeach-abc123.westeurope.azurecontainerapps.io
azd deploy api
```

### Subsequent deployments

```bash
azd deploy          # redeploy both services
azd deploy api      # redeploy only the API
azd deploy frontend # redeploy only the frontend
```

### How Aspire wires it up

The AppHost `Program.cs` expresses the full topology:

```csharp
var api = builder.AddProject<Projects.WeatherActivityMatcher_Api>("api");

builder.AddNpmApp("frontend", "../../frontend", scriptName: "dev")
    .WithReference(api)                                          // service discovery
    .WithEnvironment("VITE_API_BASE_URL", api.GetEndpoint("http")) // dev Vite proxy
    .WithEnvironment("API_UPSTREAM", api.GetEndpoint("http"))    // nginx template (prod)
    .WithHttpEndpoint(port: 5173, env: "VITE_PORT")
    .PublishAsDockerFile();  // ← tells azd to use frontend/Dockerfile when deploying
```

- `WithReference(api)` → Aspire injects the API's URL into the frontend container at runtime
- `WithEnvironment("API_UPSTREAM", ...)` → nginx's `${API_UPSTREAM}` template variable is set automatically
- `PublishAsDockerFile()` → in dev the npm script runs; in production the Dockerfile is used

No fly.toml, no hand-written YAML. Aspire generates the infrastructure.

---

## Option B — Fly.io (simpler, no Azure required)

Fly.io has a generous free tier and needs only Docker — no Aspire tooling at deploy time.

### Prerequisites

```bash
curl -L https://fly.io/install.sh | sh
fly auth login   # opens browser, sign up free
```

### 1 — Deploy the API

The backend Dockerfile uses `./backend` as its build context (it copies the
ServiceDefaults project as a sibling). Deploy from that directory:

```bash
fly apps create weather-matcher-api
fly volumes create sqlite_data --app weather-matcher-api --region ams --size 1

cd backend
fly deploy --app weather-matcher-api \
           --dockerfile WeatherActivityMatcher.Api/Dockerfile \
           --config ../fly.api.toml \
           --remote-only
cd ..

# Verify
curl https://weather-matcher-api.fly.dev/health
```

### 2 — Deploy the frontend

```bash
fly apps create weather-matcher-ui
fly deploy --app weather-matcher-ui --config fly.frontend.toml --remote-only

# Verify
curl https://weather-matcher-ui.fly.dev/nginx-health
```

### 3 — Set CORS (one time)

```bash
fly secrets set --app weather-matcher-api \
  CORS_ORIGINS=https://weather-matcher-ui.fly.dev
```

Open the app: `fly open --app weather-matcher-ui`

### Subsequent deployments

```bash
# API (from backend/ dir)
cd backend && fly deploy --app weather-matcher-api \
  --dockerfile WeatherActivityMatcher.Api/Dockerfile \
  --config ../fly.api.toml --remote-only && cd ..

# Frontend
fly deploy --app weather-matcher-ui --config fly.frontend.toml --remote-only
```

---

## Local — Docker Compose

```bash
docker compose up --build
# App:  http://localhost
# API:  http://localhost:5000
# Docs: http://localhost:5000/swagger
```

## Local — Dev mode (Aspire dashboard)

```bash
cd backend/WeatherActivityMatcher.AppHost
dotnet run
# Aspire dashboard: https://localhost:15888
# Frontend (Vite): http://localhost:5173
# API:             http://localhost:5000
```

---

## Smoke-test checklist

After any deployment:

| Check | Expected |
|-------|----------|
| `GET /health` | `{"status":"Healthy"}` |
| `GET /swagger` | Swagger UI |
| `GET /api/v1/activities` | `[]` |
| Open frontend → Settings | Location form shows Amsterdam defaults |
| Add BBQ activity → "Refresh Now" on Dashboard | Match windows appear |
| Bell icon | Shows unread count after matching |
