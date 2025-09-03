#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
dotnet run --project "$SCRIPT_DIR/../book.csproj" -- --chapter 15 --mode benchmark | cat


