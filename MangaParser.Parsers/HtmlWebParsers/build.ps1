[CmdletBinding()]
param (
    # Specifies a build configuration.
    [Parameter(Mandatory = $true,
        Position = 0,
        HelpMessage = "Build configuration.")]
    [Alias("c")]
    [ValidateNotNullOrEmpty()]
    [string]
    $Configuration,

    # Specifies a path to one or more locations.
    [Parameter(Mandatory = $false,
        Position = 1,
        ParameterSetName = "Path",
        ValueFromPipeline = $true,
        ValueFromPipelineByPropertyName = $true,
        HelpMessage = "Path to one or more locations.")]
    [Alias("PSPath", "p")]
    [ValidateNotNullOrEmpty()]
    [string[]]
    $Path = $null
)

$excludedPaths = "bin", "obj"
$startDir = Get-Location

if ($null -eq $Path) {
    $Path = Get-ChildItem -Exclude $excludedPaths -Directory -Force | Select-Object -ExpandProperty FullName
}

$Path | ForEach-Object {
    Set-Location -Path $_
    dotnet build -c $Configuration
}

Set-Location $startDir