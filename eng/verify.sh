#!/usr/bin/env sh
set -eu

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)

if command -v pwsh >/dev/null 2>&1; then
  exec pwsh -NoProfile -File "$SCRIPT_DIR/verify.ps1" "$@"
fi

printf '%s\n' "PowerShell (pwsh) is required to call the repository-owned verification entry point." >&2
exit 1
