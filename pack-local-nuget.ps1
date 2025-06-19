param(
    [string]$Project
)

# PowerShell script to build and pack one project or all src projects into ./local-nuget using MinVer for versioning

# Ensure output directory exists
$localNuget = "./local-nuget"
if (-not (Test-Path $localNuget)) {
    New-Item -ItemType Directory -Path $localNuget | Out-Null
}

# Install MinVer tool if not already installed
if (-not (Get-Command minver -ErrorAction SilentlyContinue)) {
    dotnet tool install --global minver-cli
    $env:PATH += ";$env:USERPROFILE\.dotnet\tools"
}

if ($Project) {
    if (-not (Test-Path $Project)) {
        Write-Host "Project file '$Project' not found."
        exit 1
    }
    Write-Host "Packing $Project..."
    dotnet pack "$Project" --configuration Release --output $localNuget
} else {
    Write-Host "Packing all projects in ./src..."
    Get-ChildItem -Path ./src -Filter *.csproj -Recurse | ForEach-Object {
        $proj = $_.FullName
        Write-Host "Packing $proj..."
        dotnet pack "$proj" --configuration Release --output $localNuget
    }
}

Write-Host "NuGet packages are available in $localNuget"