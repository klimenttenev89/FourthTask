#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

echo "=== NorthwindTraders — Build ==="
echo "Root: $ROOT"
echo ""

dotnet build "$ROOT/NorthwindTraders.sln" --configuration Release

echo ""
echo "Build succeeded."
