param(
    [ValidateSet("PreCommit", "PrePush", "Pr", "Main")]
    [string]$Profile = "PrePush"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $repoRoot "EngineeringModel.Reference.sln"

$unitTests = @(
    "tests/EngineeringModel.Modules.Projects.UnitTests/EngineeringModel.Modules.Projects.UnitTests.csproj",
    "tests/EngineeringModel.Modules.WorkItems.UnitTests/EngineeringModel.Modules.WorkItems.UnitTests.csproj"
)
$architectureTests = "tests/EngineeringModel.ArchitectureTests/EngineeringModel.ArchitectureTests.csproj"
$integrationTests = "tests/EngineeringModel.Api.IntegrationTests/EngineeringModel.Api.IntegrationTests.csproj"
$templateSmoke = Join-Path $repoRoot "eng/template-smoke.ps1"

function Invoke-DotNet {
    param([Parameter(Position = 0, ValueFromRemainingArguments = $true)][string[]]$Arguments)

    Write-Host "dotnet $($Arguments -join ' ')" -ForegroundColor Cyan
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet command failed with exit code $LASTEXITCODE"
    }
}

function Invoke-TestProject {
    param([string]$Project)
    Invoke-DotNet test $Project -c Release --no-build --no-restore --verbosity minimal
}

Push-Location $repoRoot
try {
    Write-Host "=== Engineering Model Verification: $Profile ===" -ForegroundColor Green

    Invoke-DotNet restore $solution
    Invoke-DotNet build $solution -c Release --no-restore

    if ($Profile -eq "PreCommit") {
        Write-Host "PreCommit verification passed." -ForegroundColor Green
        exit 0
    }

    foreach ($testProject in $unitTests) {
        Invoke-TestProject $testProject
    }

    Invoke-TestProject $architectureTests

    if ($Profile -in @("Pr", "Main")) {
        Invoke-TestProject $integrationTests
    }

    if ($Profile -eq "Main") {
        Write-Host "Running reusable-template smoke verification..." -ForegroundColor Cyan
        & $templateSmoke
        if ($LASTEXITCODE -ne 0) {
            throw "Template smoke verification failed with exit code $LASTEXITCODE"
        }
    }

    Write-Host "Verification passed: $Profile" -ForegroundColor Green
}
finally {
    Pop-Location
}
