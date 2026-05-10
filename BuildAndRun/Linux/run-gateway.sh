#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

echo "=== Gateway -> http://localhost:5000 (Swagger: http://localhost:5000/swagger) ==="
echo ""

dotnet run --project "$ROOT/src/Gateway" --configuration Release
