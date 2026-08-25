$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot

Push-Location $repoRoot
try {
    git config core.hooksPath .githooks
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to configure Git hooks path."
    }

    Write-Host "Git hooks configured: .githooks" -ForegroundColor Green
}
finally {
    Pop-Location
}
