$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$verifyScript = Join-Path $PSScriptRoot "verify.ps1"
$domainProject = Join-Path $repoRoot "src/Modules/WorkItems/EngineeringModel.Modules.WorkItems.Domain/EngineeringModel.Modules.WorkItems.Domain.csproj"
$probeFile = Join-Path $repoRoot "src/Modules/WorkItems/EngineeringModel.Modules.WorkItems.Domain/ForbiddenDependencyProbe.cs"
$backup = Get-Content $domainProject -Raw
$utf8NoBom = New-Object System.Text.UTF8Encoding -ArgumentList $false

function Write-Utf8NoBom {
    param([string]$Path, [string]$Content)
    [System.IO.File]::WriteAllText($Path, $Content, $utf8NoBom)
}

$forbiddenReference = '    <ProjectReference Include="../../Projects/EngineeringModel.Modules.Projects.Infrastructure/EngineeringModel.Modules.Projects.Infrastructure.csproj" />'

Write-Host "1/3 Baseline verification (must be green before the demonstration)..." -ForegroundColor Cyan
& $verifyScript -Profile PrePush

$violationDetected = $false
try {
    $modified = $backup.Replace("</Project>", "  <ItemGroup>`n$forbiddenReference`n  </ItemGroup>`n</Project>")
    Write-Utf8NoBom -Path $domainProject -Content $modified

    @'
using EngineeringModel.Modules.Projects.Infrastructure;

namespace EngineeringModel.Modules.WorkItems.Domain;

internal static class ForbiddenDependencyProbe
{
    internal static Type ForbiddenType => typeof(SqliteProjectRepository);
}
'@ | ForEach-Object { Write-Utf8NoBom -Path $probeFile -Content $_ }

    Write-Host "2/3 Temporary WorkItems.Domain -> Projects.Infrastructure violation introduced." -ForegroundColor Yellow
    Write-Host "Expected result: PrePush verification is rejected by architecture tests." -ForegroundColor Yellow

    try {
        & $verifyScript -Profile PrePush
    }
    catch {
        $violationDetected = $true
        Write-Host "Architecture violation rejected as expected." -ForegroundColor Green
    }

    if (-not $violationDetected) {
        throw "The deliberate architecture violation was not detected."
    }
}
finally {
    Write-Utf8NoBom -Path $domainProject -Content $backup
    if (Test-Path $probeFile) {
        Remove-Item $probeFile -Force
    }
}

Write-Host "3/3 Temporary files reverted. Verifying green state again..." -ForegroundColor Cyan
& $verifyScript -Profile PrePush
Write-Host "Architecture enforcement demonstration completed successfully." -ForegroundColor Green
