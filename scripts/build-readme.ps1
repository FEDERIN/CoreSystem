$repoRoot = Resolve-Path "$PSScriptRoot\.."
$docsPath = Join-Path $repoRoot "docs\Cache"
$outputFile = Join-Path $repoRoot "src\Core.Cache\README.md"

Write-Host "Generating README.md..."

# Obtener todos los archivos Markdown ordenados por nombre
$files = Get-ChildItem -Path $docsPath -Filter *.md |
         Sort-Object Name

# Crear el directorio de salida si no existe
$outputDirectory = Split-Path $outputFile
if (!(Test-Path $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory | Out-Null
}

# Limpiar el archivo de salida
Set-Content -Path $outputFile -Value ""

foreach ($file in $files) {

    Write-Host "Adding $($file.Name)..."

    Get-Content -Path $file.FullName | Add-Content -Path $outputFile

    Add-Content -Path $outputFile ""
    Add-Content -Path $outputFile "---"
    Add-Content -Path $outputFile ""
}

Write-Host ""
Write-Host "README generated successfully!"
Write-Host "Output: $outputFile"