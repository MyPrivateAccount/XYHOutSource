param($installPath, $toolsPath, $package, $project)

if ($PSVersionTable.PSVersion.Major -lt 3)
{
    # This section needs to support PS2 syntax
    # Use $toolsPath because PS2 does not support $PSScriptRoot
    $env:PSModulePath = $env:PSModulePath + ';$toolsPath'

    # Import a "dummy" module that contains matching functions that throw on PS2
    Import-Module (Join-Path $toolsPath 'EntityFrameworkCore.PowerShell2.psd1') -DisableNameChecking

    throw "PowerShell version $($PSVersionTable.PSVersion) is not supported. Please upgrade PowerShell to 3.0 or " +
        'greater and restart Visual Studio.'
}
else
{
    if (Get-Module 'EntityFrameworkCore')
    {
        Remove-Module 'EntityFrameworkCore'
    }

    Import-Module (Join-Path $PSScriptRoot 'EntityFrameworkCore.psd1') -DisableNameChecking
}

