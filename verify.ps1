<#
.SYNOPSIS
    The single machine check for this repository. Exit code 0 means the Definition of Done
    is met for the requested stage; any other code means it is not.

.DESCRIPTION
    This script is the hard evidence a /goal run must produce. The /goal evaluator reads only
    the transcript, so the condition points at this script and its exit code, never at prose.

    Checks, in order:
      1. No BLOCKED file in the repository root.
      2. Release build with no warnings (TreatWarningsAsErrors is on in Directory.Build.props).
      3. Snake.Core has no dependency on WPF (NFR-01).
      4. Tests pass and line coverage of Snake.Core is at or above the threshold.
      5. At least -MinTests tests actually ran.
      6. Every tracker task of the requested stage has status 'done'.

.PARAMETER Stage
    Which tracker stage to check: 1, 2, 3, or 0 for all stages. Default 0.

.PARAMETER MinTests
    Minimum number of tests that must run. Guards against a single test that games coverage.

.PARAMETER Threshold
    Minimum line coverage of Snake.Core, in percent.

.EXAMPLE
    ./verify.ps1 -Stage 1 -MinTests 25
#>
[CmdletBinding()]
param(
    [ValidateRange(0, 3)]
    [int] $Stage = 0,

    [int] $MinTests = 1,

    [ValidateRange(0, 100)]
    [int] $Threshold = 80,

    # Git ref the protected files are compared against. Pass '' to skip the check.
    [string] $Baseline = 'baseline'
)

# 'Continue', not 'Stop', on purpose. dotnet and git write progress and warnings to stderr;
# with 'Stop' PowerShell turns those into terminating errors and the script dies mid-check.
# Every external call below is judged by its exit code instead.
$ErrorActionPreference = 'Continue'
Set-StrictMode -Version Latest

$root = $PSScriptRoot
$failures = New-Object System.Collections.Generic.List[string]

function Write-Step([string] $text) {
    Write-Host ''
    Write-Host "==> $text" -ForegroundColor Cyan
}

function Add-Failure([string] $text) {
    $script:failures.Add($text)
    Write-Host "FAIL: $text" -ForegroundColor Red
}

function Write-Ok([string] $text) {
    Write-Host "ok: $text" -ForegroundColor Green
}

# ---------------------------------------------------------------- 1. BLOCKED

Write-Step 'Checking for a BLOCKED file'

$blockedPath = Join-Path $root 'BLOCKED'
if (Test-Path $blockedPath) {
    $reason = (Get-Content $blockedPath -Raw -Encoding UTF8).Trim()
    Add-Failure "BLOCKED file is present. Reason: $reason"
}
else {
    Write-Ok 'no BLOCKED file'
}

# ------------------------------------------------- 2. Protected files untouched

# The shortest path to a green check is to weaken the check. These files define the rules,
# so they must stay exactly as they were when the baseline was tagged.
$protected = @('verify.ps1', 'Directory.Build.props', 'REQUIREMENTS.md')

Write-Step "Checking protected files against '$Baseline'"

if ([string]::IsNullOrWhiteSpace($Baseline)) {
    Write-Host 'skipped: no baseline given' -ForegroundColor Yellow
}
else {
    & git -C $root rev-parse --verify --quiet "$Baseline^{commit}" > $null 2>&1

    if ($LASTEXITCODE -ne 0) {
        Add-Failure "Baseline ref '$Baseline' does not exist. Create it with: git tag $Baseline"
    }
    else {
        # No 2>&1 here: git writes line-ending warnings to stderr and they would be
        # mistaken for file names.
        $changed = @(& git -C $root diff --name-only $Baseline -- $protected |
            Where-Object { $_ -and $_.Trim() })

        if ($LASTEXITCODE -ne 0) {
            Add-Failure "git diff against '$Baseline' failed with code $LASTEXITCODE"
        }
        elseif ($changed.Count -gt 0) {
            foreach ($file in $changed) {
                Add-Failure "Protected file changed since '$Baseline': $file"
            }
        }
        else {
            Write-Ok 'protected files are unchanged'
        }
    }
}

# ------------------------------------------------------------------ 3. Build

Write-Step 'Building (Release, warnings are errors)'

$buildOutput = & dotnet build (Join-Path $root 'Snake.slnx') -c Release --nologo 2>&1
$buildOutput | ForEach-Object { Write-Host $_ }

if ($LASTEXITCODE -ne 0) {
    Add-Failure "dotnet build exited with code $LASTEXITCODE"
}
else {
    Write-Ok 'build is clean'
}

# ------------------------------------------------- 3. Snake.Core must be UI-free

Write-Step 'Checking that Snake.Core does not depend on WPF (NFR-01)'

$corePath = Join-Path $root 'src/Snake.Core/Snake.Core.csproj'
if (-not (Test-Path $corePath)) {
    Add-Failure "Snake.Core.csproj not found at $corePath"
}
else {
    $core = Get-Content $corePath -Raw -Encoding UTF8

    if ($core -match '<UseWPF>\s*true\s*</UseWPF>') {
        Add-Failure 'Snake.Core.csproj sets UseWPF'
    }
    if ($core -match '<UseWindowsForms>\s*true\s*</UseWindowsForms>') {
        Add-Failure 'Snake.Core.csproj sets UseWindowsForms'
    }
    if ($core -match '-windows') {
        Add-Failure 'Snake.Core targets a Windows-only framework'
    }
    if ($core -match 'Snake\.Wpf') {
        Add-Failure 'Snake.Core references Snake.Wpf'
    }

    $coreSources = @(Get-ChildItem -Path (Join-Path $root 'src/Snake.Core') -Filter '*.cs' -Recurse -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' })

    # An empty module makes coverlet report 100%, which would make the threshold meaningless.
    if ($coreSources.Count -eq 0) {
        Add-Failure 'Snake.Core has no source files, so the coverage threshold proves nothing'
    }

    $uiUsings = $coreSources |
        Select-String -Pattern '^\s*using\s+(System\.Windows|System\.Drawing|PresentationCore)' -ErrorAction SilentlyContinue

    if ($uiUsings) {
        foreach ($hit in $uiUsings) {
            Add-Failure "UI namespace used in Snake.Core: $($hit.Path):$($hit.LineNumber)"
        }
    }

    if ($failures.Count -eq 0 -or -not ($failures -match 'Snake\.Core')) {
        Write-Ok 'Snake.Core is free of UI dependencies'
    }
}

# ------------------------------------------------------- 4+5. Tests, coverage

Write-Step "Running tests (coverage threshold $Threshold%, at least $MinTests tests)"

$resultsDir = Join-Path $root 'TestResults'
if (Test-Path $resultsDir) {
    Remove-Item $resultsDir -Recurse -Force
}

$testProject = Join-Path $root 'tests/Snake.Core.Tests/Snake.Core.Tests.csproj'

if (-not (Test-Path $testProject)) {
    Add-Failure "Test project not found at $testProject"
}
else {
    $testArgs = @(
        'test', $testProject,
        '-c', 'Release',
        '--nologo',
        '--results-directory', $resultsDir,
        '--logger', 'trx;LogFileName=results.trx',
        '/p:CollectCoverage=true',
        '/p:CoverletOutputFormat=json',
        "/p:Include=[Snake.Core]*",
        "/p:Threshold=$Threshold",
        '/p:ThresholdType=line',
        '/p:ThresholdStat=total'
    )

    $testOutput = & dotnet @testArgs 2>&1
    $testOutput | ForEach-Object { Write-Host $_ }
    $testExitCode = $LASTEXITCODE

    if ($testExitCode -ne 0) {
        Add-Failure "dotnet test exited with code $testExitCode (failing tests or coverage below $Threshold%)"
    }
    else {
        Write-Ok "tests pass and coverage is at or above $Threshold%"
    }

    $trx = Get-ChildItem -Path $resultsDir -Filter '*.trx' -Recurse -ErrorAction SilentlyContinue |
        Select-Object -First 1

    if (-not $trx) {
        Add-Failure 'No .trx result file was produced, so the test count cannot be checked'
    }
    else {
        [xml] $trxXml = Get-Content $trx.FullName -Raw -Encoding UTF8
        $counters = $trxXml.TestRun.ResultSummary.Counters
        $total = [int] $counters.total
        $passed = [int] $counters.passed
        $failed = [int] $counters.failed

        Write-Host "tests: total=$total passed=$passed failed=$failed"

        if ($failed -gt 0) {
            Add-Failure "$failed test(s) failed"
        }
        if ($total -lt $MinTests) {
            Add-Failure "only $total test(s) ran, at least $MinTests required"
        }
        if ($failed -eq 0 -and $total -ge $MinTests) {
            Write-Ok "$total tests ran, none failed"
        }
    }
}

# ---------------------------------------------------------------- 6. Tracker

if ($Stage -eq 0) {
    Write-Step 'Checking tracker: every task must be done'
}
else {
    Write-Step "Checking tracker: every stage $Stage task must be done"
}

$trackerPath = Join-Path $root 'tracker.md'

if (-not (Test-Path $trackerPath)) {
    Add-Failure "tracker.md not found at $trackerPath"
}
else {
    $currentStage = 0
    $checked = 0
    $notDone = New-Object System.Collections.Generic.List[string]

    foreach ($line in (Get-Content $trackerPath -Encoding UTF8)) {
        if ($line -match '^<!--\s*stage:\s*(\d+)\s*-->') {
            $currentStage = [int] $Matches[1]
            continue
        }

        if ($line -match '^\|\s*(T\d+)\s*\|.*\|\s*(todo|done|blocked)\s*\|\s*$') {
            $id = $Matches[1]
            $status = $Matches[2]

            if ($Stage -ne 0 -and $currentStage -ne $Stage) {
                continue
            }

            $checked++
            if ($status -ne 'done') {
                $notDone.Add("$id is '$status'")
            }
        }
    }

    if ($checked -eq 0) {
        Add-Failure 'No tracker tasks matched, so nothing was checked. Is the tracker format intact?'
    }
    elseif ($notDone.Count -gt 0) {
        Add-Failure "$($notDone.Count) of $checked task(s) not done: $($notDone -join ', ')"
    }
    else {
        Write-Ok "all $checked tracked task(s) are done"
    }
}

# ----------------------------------------------------------------- Verdict

Write-Host ''
Write-Host ('-' * 60)

if ($failures.Count -gt 0) {
    Write-Host "VERIFY FAILED: $($failures.Count) problem(s)" -ForegroundColor Red
    foreach ($failure in $failures) {
        Write-Host "  - $failure" -ForegroundColor Red
    }
    Write-Host ('-' * 60)
    exit 1
}

Write-Host 'VERIFY PASSED' -ForegroundColor Green
Write-Host ('-' * 60)
exit 0
