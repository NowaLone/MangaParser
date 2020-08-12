[CmdletBinding()]
param (
    # Specifies a path to one or more locations. Wildcards are permitted.
    [Parameter(Mandatory = $true,
        Position = 0,
        ValueFromPipeline = $true,
        ValueFromPipelineByPropertyName = $true,
        HelpMessage = "Path to one or more locations.")]
    [ValidateNotNullOrEmpty()]
    [SupportsWildcards()]
    [string[]]
    $Path
)

$assemblyPattern = '<AssemblyVersion>1.0.0.0</AssemblyVersion>'
$filePattern = '<FileVersion>1.0.0.0</FileVersion>'
$nugetPattern = '<Version>1.0.0</Version>'

$major = $env:VERSION_MAJOR
$minor = $env:VERSION_MINOR
$build = $env:VERSION_BUILD
$rev = $env:VERSION_REV

$rc = 'rc'
$beta = 'beta'
$alpha = 'alpha'

$newAssemblyPattern = "<AssemblyVersion>$major.0.0.0</AssemblyVersion>"
$newFilePattern = "<FileVersion>$major.$minor.$build.$rev</FileVersion>"
$newNugetPattern = "<Version>$major.$minor.$build</Version>"

if ($env:RELEASE -ne $true -Or $env:GITHUB_REF -ne 'master' ) {
    switch ($env:GITHUB_REF) {
        'master' { $newNugetPattern = "<Version>$major.$minor.$build-$rc.$rev</Version>"; Break }
        'Dev' { $newNugetPattern = "<Version>$major.$minor.$build-$beta.$rev</Version>"; Break }
        Default { $newNugetPattern = "<Version>$major.$minor.$build-$alpha.$rev</Version>"; Break }
    }
}

foreach ($item in Resolve-Path $Path) {
    (Get-Content -Path $item) | ForEach-Object { $_ -replace $assemblyPattern, $newAssemblyPattern -replace $filePattern, $newFilePattern -replace $nugetPattern, $newNugetPattern } | Set-Content -Path $item
}