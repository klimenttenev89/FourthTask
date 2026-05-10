#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

echo "=== CustomerService -> http://localhost:5001 ==="
echo ""

dotnet run --project "$ROOT/src/CustomerService" --configuration Release
