#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

echo "=== Frontend -> http://localhost:5002 ==="
echo ""

dotnet run --project "$ROOT/src/Frontend" --configuration Release
