<#
.SYNOPSIS
    Builds the game and starts it.

.DESCRIPTION
    A convenience launcher, not a check. The machine Definition of Done lives in verify.ps1.

    Steps:
      1. Build src/Snake.Wpf in the requested configuration (skipped with -NoBuild).
      2. Start the built Snake.Wpf.exe.

    Exit code 0 means the game was started. Any other code means it was not.

.PARAMETER Configuration
    Build configuration: Release (default) or Debug.

.PARAMETER NoBuild
    Start the game that is already built and skip the build step.

.PARAMETER Wait
    Block until the game window is closed and return the exit code of the game.
    Without it the script returns as soon as the game has started.

.EXAMPLE
    ./run.ps1

.EXAMPLE
    ./run.ps1 -Configuration Debug -Wait
#>
[CmdletBinding()]
param(
    [ValidateSet('Release', 'Debug')]
    [string] $Configuration = 'Release',

    [switch] $NoBuild,

    [switch] $Wait
)

# 'Continue', not 'Stop', on purpose: dotnet writes progress and warnings to stderr, and with
# 'Stop' PowerShell turns those into terminating errors. Every external call is judged by its
# exit code instead.
$ErrorActionPreference = 'Continue'
Set-StrictMode -Version Latest

$root = $PSScriptRoot
$project = Join-Path $root 'src/Snake.Wpf/Snake.Wpf.csproj'

function Write-Step([string] $text) {
    Write-Host ''
    Write-Host "==> $text" -ForegroundColor Cyan
}

function Write-Ok([string] $text) {
    Write-Host "ok: $text" -ForegroundColor Green
}

function Exit-WithFailure([string] $text) {
    Write-Host "FAIL: $text" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $project)) {
    Exit-WithFailure "Project not found at $project"
}

# The target framework is read from the project file, so the path below cannot drift from it.
if ((Get-Content $project -Raw -Encoding UTF8) -notmatch '<TargetFramework>([^<]+)</TargetFramework>') {
    Exit-WithFailure "No TargetFramework in $project"
}

$targetFramework = $Matches[1]
$exe = Join-Path $root "src/Snake.Wpf/bin/$Configuration/$targetFramework/Snake.Wpf.exe"

if ($NoBuild) {
    Write-Step 'Building'
    Write-Host 'skipped: -NoBuild was given' -ForegroundColor Yellow
}
else {
    Write-Step "Building ($Configuration, warnings are errors)"

    & dotnet build $project -c $Configuration --nologo

    if ($LASTEXITCODE -ne 0) {
        Exit-WithFailure "dotnet build exited with code $LASTEXITCODE"
    }

    Write-Ok 'build is clean'
}

if (-not (Test-Path $exe)) {
    Exit-WithFailure "Game not found at $exe. Run without -NoBuild to build it."
}

Write-Step 'Starting the game'

$game = Start-Process -FilePath $exe -PassThru

if (-not $Wait) {
    Write-Ok "started: $exe (process id $($game.Id))"
    exit 0
}

$game.WaitForExit()
Write-Ok "the game exited with code $($game.ExitCode)"
exit $game.ExitCode
