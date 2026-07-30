param(
    [ValidateSet("Status", "Script", "Apply")]
    [string]$Mode = "Status",
    [ValidateSet("Development", "Staging", "Production")]
    [string]$Environment = "Development",
    [string]$Output = "artifacts/database-migration.sql"
)

$ErrorActionPreference = "Stop"
$projectPath = Join-Path $PSScriptRoot "..\ProjectBase.csproj"
$projectPath = [System.IO.Path]::GetFullPath($projectPath)
$env:ASPNETCORE_ENVIRONMENT = $Environment

dotnet build $projectPath -c Release --no-restore
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

if ($Mode -eq "Status") {
    dotnet ef migrations list `
        --project $projectPath `
        --startup-project $projectPath `
        --configuration Release `
        --no-build
    exit $LASTEXITCODE
}

if ($Mode -eq "Script") {
    $outputPath = [System.IO.Path]::GetFullPath(
        (Join-Path (Split-Path $projectPath -Parent) $Output))
    $outputDirectory = Split-Path $outputPath -Parent
    New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null

    dotnet ef migrations script `
        --idempotent `
        --output $outputPath `
        --project $projectPath `
        --startup-project $projectPath `
        --configuration Release `
        --no-build
    exit $LASTEXITCODE
}

dotnet ef database update `
    --project $projectPath `
    --startup-project $projectPath `
    --configuration Release `
    --no-build
exit $LASTEXITCODE
