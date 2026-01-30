<#
.SYNOPSIS
    Setup script to rename this Blazor template project to your own project name.

.DESCRIPTION
    This script renames the solution, project folder, and .csproj file.
    Namespaces in code files are NOT changed - they remain as Deneblab.BlazorDaisy.

.PARAMETER ProjectName
    The new project name (e.g., "MyApp")

.PARAMETER NoCleanup
    Skip deleting template files after setup

.EXAMPLE
    .\setup.ps1 -ProjectName "MyApp"
    .\setup.ps1 -ProjectName "MyApp" -NoCleanup
#>

param(
    [string]$ProjectName,
    [switch]$NoCleanup
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Colors for output
function Write-Step { param($msg) Write-Host ">> $msg" -ForegroundColor Cyan }
function Write-Success { param($msg) Write-Host "   $msg" -ForegroundColor Green }
function Write-Warning { param($msg) Write-Host "   $msg" -ForegroundColor Yellow }

# Current names
$OldName = "Deneblab.BlazorDaisy"

# Read from template.json if exists and no parameters provided
$TemplateJsonPath = Join-Path $ScriptDir "src\$OldName\template.json"
if ((Test-Path $TemplateJsonPath) -and -not $ProjectName) {
    $template = Get-Content $TemplateJsonPath | ConvertFrom-Json
    if ($template.project.name -and $template.project.name -ne "MyNewProject") {
        $ProjectName = $template.project.name
        Write-Step "Using settings from template.json"
    }
}

# Prompt if still not provided
if (-not $ProjectName) {
    Write-Host ""
    Write-Host "=== Blazor DaisyUI Template Setup ===" -ForegroundColor Magenta
    Write-Host ""
    Write-Host "This script will rename:" -ForegroundColor Yellow
    Write-Host "  - Solution file"
    Write-Host "  - Project folder"
    Write-Host "  - .csproj file"
    Write-Host ""
    Write-Host "Namespaces in code files will NOT be changed." -ForegroundColor Yellow
    Write-Host ""
    $ProjectName = Read-Host "Enter new project name (e.g., MyApp)"
    if (-not $ProjectName) {
        Write-Host "Project name is required." -ForegroundColor Red
        exit 1
    }
}

# Validate project name
if ($ProjectName -match '[^\w\.]') {
    Write-Host "Project name can only contain letters, numbers, and dots." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Configuration:" -ForegroundColor Yellow
Write-Host "  Old Name:  $OldName"
Write-Host "  New Name:  $ProjectName"
Write-Host ""

$confirm = Read-Host "Proceed with rename? (y/N)"
if ($confirm -ne "y" -and $confirm -ne "Y") {
    Write-Host "Aborted." -ForegroundColor Yellow
    exit 0
}

Write-Host ""

# Step 1: Update solution file references
Write-Step "Updating solution file..."
$slnPath = Join-Path $ScriptDir "src\$OldName.sln"
if (Test-Path $slnPath) {
    $content = Get-Content $slnPath -Raw
    $newContent = $content -replace [regex]::Escape($OldName), $ProjectName
    Set-Content -Path $slnPath -Value $newContent -NoNewline
    Write-Success "Updated solution references"
}

# Step 2: Rename .csproj file
Write-Step "Renaming project file..."
$csprojPath = Join-Path $ScriptDir "src\$OldName\$OldName.csproj"
if (Test-Path $csprojPath) {
    Rename-Item -Path $csprojPath -NewName "$ProjectName.csproj"
    Write-Success "Renamed to $ProjectName.csproj"
}

# Step 3: Rename project folder
Write-Step "Renaming project folder..."
$oldFolderPath = Join-Path $ScriptDir "src\$OldName"
$newFolderPath = Join-Path $ScriptDir "src\$ProjectName"
if (Test-Path $oldFolderPath) {
    Rename-Item -Path $oldFolderPath -NewName $ProjectName
    Write-Success "Renamed folder to $ProjectName"
}

# Step 4: Rename solution file
Write-Step "Renaming solution file..."
if (Test-Path $slnPath) {
    Rename-Item -Path $slnPath -NewName "$ProjectName.sln"
    Write-Success "Renamed to $ProjectName.sln"
}

# Step 5: Update README.md project references
Write-Step "Updating README.md..."
$readmePath = Join-Path $ScriptDir "README.md"
if (Test-Path $readmePath) {
    $content = Get-Content $readmePath -Raw
    $newContent = $content -replace [regex]::Escape($OldName), $ProjectName
    Set-Content -Path $readmePath -Value $newContent -NoNewline
    Write-Success "Updated README.md"
}

# Cleanup
if (-not $NoCleanup) {
    Write-Step "Cleaning up template files..."

    $templateJson = Join-Path $ScriptDir "src\$ProjectName\template.json"
    if (Test-Path $templateJson) {
        Remove-Item $templateJson -Force
        Write-Success "Removed template.json"
    }

    $templateMd = Join-Path $ScriptDir "src\$ProjectName\TEMPLATE.md"
    if (Test-Path $templateMd) {
        Remove-Item $templateMd -Force
        Write-Success "Removed TEMPLATE.md"
    }

    Write-Warning "Note: Run 'Remove-Item setup.ps1, setup.sh' to remove setup scripts"
}

Write-Host ""
Write-Host "=== Setup Complete ===" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. cd src\$ProjectName"
Write-Host "  2. dotnet restore"
Write-Host "  3. dotnet run"
Write-Host ""
