$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("engineering-model-template-smoke-" + [Guid]::NewGuid().ToString("N"))
$templateName = "TemplateSmoke"
$installed = $false

function Invoke-DotNet {
    param([Parameter(Position = 0, ValueFromRemainingArguments = $true)][string[]]$Arguments)

    Write-Host "dotnet $($Arguments -join ' ')" -ForegroundColor Cyan
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet command failed with exit code $LASTEXITCODE"
    }
}

try {
    Write-Host "=== Template Smoke Test ===" -ForegroundColor Green
    Write-Host "Installing repository as a local dotnet template..."

    Invoke-DotNet new install $repoRoot --force
    $installed = $true

    Invoke-DotNet new engmodel-mm -n $templateName -o $tempRoot

    $generatedVerify = Join-Path $tempRoot "eng/verify.ps1"
    if (-not (Test-Path $generatedVerify)) {
        throw "Generated template does not contain eng/verify.ps1"
    }

    Write-Host "Running generated repository verification..." -ForegroundColor Cyan
    & $generatedVerify -Profile PrePush
    if ($LASTEXITCODE -ne 0) {
        throw "Generated template verification failed with exit code $LASTEXITCODE"
    }

    Write-Host "Template smoke test passed." -ForegroundColor Green
}
finally {
    if ($installed) {
        & dotnet new uninstall $repoRoot *> $null
    }

    if (Test-Path $tempRoot) {
        Remove-Item $tempRoot -Recurse -Force
    }
}
