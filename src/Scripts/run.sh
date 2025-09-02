#!/usr/bin/env bash
set -euo pipefail

CHAPTER=${1:-1}
MODE=${2:-b}

dotnet run --project ../book.csproj -- -c "$CHAPTER" -m "$MODE"


