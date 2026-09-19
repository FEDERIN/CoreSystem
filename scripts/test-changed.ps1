$ErrorActionPreference = "Stop"

$solution = "CoreSystem.sln"
$configuration = "Release"

$changedFiles = @(
    git diff --name-only HEAD
    git ls-files --others --exclude-standard
) | Sort-Object -Unique

if (-not $changedFiles) {
    Write-Host "No changes detected."
    exit 0
}

$fullSuitePatterns = @(
    '^CoreSystem\.sln$',
    '^Directory\.Packages\.props$',
    '^global\.json$',
    '^NuGet\.config$',
    '^src/Core\.Serialization/',
    '^src/Core\.Observability\.Abstractions/'
)

$requiresFullSuite = $changedFiles | Where-Object {
    $file = $_

    $fullSuitePatterns | Where-Object {
        $file -match $_
    }
}

if ($requiresFullSuite) {
    Write-Host "Shared or solution-level change detected. Running full suite."
    dotnet test $solution --configuration $configuration
    exit $LASTEXITCODE
}

function Get-ContainingProject([string]$file) {
    $directory = Split-Path $file -Parent

    while ($directory) {
        $project = Get-ChildItem $directory -Filter "*.csproj" -File `
            -ErrorAction SilentlyContinue |
            Select-Object -First 1

        if ($project) {
            return $project.FullName
        }

        $parent = Split-Path $directory -Parent

        if ($parent -eq $directory) {
            break
        }

        $directory = $parent
    }

    return $null
}

$sourceProjects = @()
$testProjects = @()

foreach ($file in $changedFiles) {
    $project = Get-ContainingProject $file

    if (-not $project) {
        continue
    }

    if ($project -match "\\tests\\") {
        $testProjects += $project
        continue
    }

    if ($project -match "\\src\\") {
        $sourceProjects += $project
    }
}

$sourceProjects = $sourceProjects | Sort-Object -Unique
$testProjects = $testProjects | Sort-Object -Unique

foreach ($sourceProject in $sourceProjects) {
    $name = [IO.Path]::GetFileNameWithoutExtension($sourceProject)

    $testProjects += Get-ChildItem tests -Recurse -File -Filter "$name.UnitTests.csproj" |
        Select-Object -ExpandProperty FullName

    $testProjects += Get-ChildItem tests -Recurse -File -Filter "$name.IntegrationTests.csproj" |
        Select-Object -ExpandProperty FullName
}

$testProjects = $testProjects | Sort-Object -Unique

if (-not $testProjects) {
    Write-Host "No affected test projects found."
    exit 0
}

foreach ($testProject in $testProjects) {
    Write-Host "Running: $testProject"

    # No --no-build: compiles only this test project and its dependencies.
    dotnet test $testProject --configuration $configuration

    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}