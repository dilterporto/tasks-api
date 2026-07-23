#!/usr/bin/env bash
set -euo pipefail

COMPOSE_FILE="docker-compose.dev-env.yml"
API_PROJECT="src/Tasks.Api"

cleanup() {
  echo ""
  echo "Stopping API..."
  kill "$API_PID" 2>/dev/null || true
  wait "$API_PID" 2>/dev/null || true
  echo "Infrastructure is still running. To stop it:"
  echo "  docker-compose -f $COMPOSE_FILE down"
}

echo "==> Starting infrastructure (db, redis, seq)..."
docker-compose -f "$COMPOSE_FILE" up -d local-db seq-logging redis-stack

echo "==> Running database migrations..."
docker-compose -f "$COMPOSE_FILE" run --rm local-db-migrations

echo "==> Starting API on http://localhost:5006 ..."
dotnet run --project "$API_PROJECT" --launch-profile http &
API_PID=$!

trap cleanup INT TERM

wait "$API_PID"
