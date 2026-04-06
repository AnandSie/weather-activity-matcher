#!/usr/bin/env bash
set -e

ROOT=$(cd "$(dirname "$0")" && pwd)

echo ""
echo "🌊 Weather Activity Matcher"
echo "==========================="
echo ""

# --- Backend ---
echo "→ Starting backend (ASP.NET Core + EF Core)..."
cd "$ROOT/backend/WeatherActivityMatcher.Api"
dotnet run --no-launch-profile &
BACKEND_PID=$!

# Wait for backend to be ready
echo "→ Waiting for backend..."
for i in $(seq 1 30); do
  if curl -sf http://localhost:5000/health >/dev/null 2>&1; then
    echo "✓ Backend ready at http://localhost:5000"
    echo "  Swagger: http://localhost:5000/swagger"
    break
  fi
  sleep 1
done

# --- Frontend ---
echo ""
echo "→ Starting frontend (Vite dev server)..."
cd "$ROOT/frontend"
npm run dev &
FRONTEND_PID=$!

echo ""
echo "==========================="
echo "✓ App running"
echo "  Frontend: http://localhost:5173"
echo "  API:      http://localhost:5000"
echo "  Swagger:  http://localhost:5000/swagger"
echo ""
echo "  Press Ctrl+C to stop."
echo "==========================="
echo ""

cleanup() {
  echo ""
  echo "Stopping services..."
  kill "$BACKEND_PID" "$FRONTEND_PID" 2>/dev/null || true
  wait "$BACKEND_PID" "$FRONTEND_PID" 2>/dev/null || true
  echo "Done."
}
trap cleanup INT TERM
wait
