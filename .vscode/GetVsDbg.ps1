<#
.SYNOPSIS
Downloads the given $Version of vsdbg for the given $RuntimeID and installs it to the given $InstallPath

.DESCRIPTION
The following script will download vsdbg and install vsdbg, the .NET Core Debugger

.PARAMETER Version
Specifies the version of vsdbg to install. Can be 'latest', VS2017U1, or a specific version string i.e. 15.0.25930.0

.PARAMETER RuntimeID
Specifies the .NET Runtime ID of the vsdbg that will be downloaded. Example: ubuntu.14.04-x64. Defaults to win7-x64.

.Parameter InstallPath
Specifies the path where vsdbg will be installed. Defaults to the directory containing this script.

.Parameter RemoveExistingOnUpgrade
Remove existing installation directory before upgrade.

.INPUTS
None. You cannot pipe inputs to GetVsDbg.

.EXAMPLE
C:\PS> .\GetVsDbg.ps1 -Version latest -RuntimeID ubuntu.14.04-x64 -InstallPath .\vsdbg

.LINK
https://github.com/Microsoft/MIEngine
#>

Param (
    [Parameter(Mandatory=$true, ParameterSetName="ByName")]
    [string]
    [ValidateSet("latest", "VS2017U1")]
    $Version,

    [Parameter(Mandatory=$true, ParameterSetName="ByNumber")]
    [string]
    [ValidatePattern("\d+\.\d+\.\d+.*")]
    $VersionNumber,

    [Parameter(Mandatory=$false)]
    [string]
    $RuntimeID,

    [Parameter(Mandatory=$false)]
    [string]
    $InstallPath = (Split-Path -Path $MyInvocation.MyCommand.Definition),

    [Parameter(Mandatory=$false)]
    [switch]
    $RemoveExistingOnUpgrade
)

# In a separate method to prevent locking zip files.
function DownloadAndExtract([string]$url, [string]$targetLocation) {
    Add-Type -assembly "System.IO.Compression.FileSystem"
    Add-Type -assembly "System.IO.Compression"
    $zipStream = (New-Object System.Net.WebClient).OpenRead($url)
    $zipArchive = New-Object System.IO.Compression.ZipArchive -ArgumentList $zipStream
    [System.IO.Compression.ZipFileExtensions]::ExtractToDirectory($zipArchive, $targetLocation)
    $zipArchive.Dispose()
    $zipStream.Dispose()
}

# Checks if the existing version is the latest version.
function IsLatest([string]$installationPath, [string]$runtimeId, [string]$version) {
    $SuccessRidFile = Join-Path -Path $installationPath -ChildPath "success_rid.txt"
    if (Test-Path $SuccessRidFile) {
        $LastRid = Get-Content -Path $SuccessRidFile
        if ($LastRid -ne $runtimeId) {
            return $false
        }
    } else {
        return $false
    }

    $SuccessVersionFile = Join-Path -Path $installationPath -ChildPath "success_version.txt"
    if (Test-Path $SuccessVersionFile) {
        $LastVersion = Get-Content -Path $SuccessVersionFile
        if ($LastVersion -ne $version) {
            return $false
        }
    } else {
        return $false
    }

    return $true
}

function WriteSuccessInfo([string]$installationPath, [string]$runtimeId, [string]$version) {
    $SuccessRidFile = Join-Path -Path $installationPath -ChildPath "success_rid.txt"
    $runtimeId | Out-File -Encoding utf8 $SuccessRidFile

    $SuccessVersionFile = Join-Path -Path $installationPath -ChildPath "success_version.txt"
    $version | Out-File -Encoding utf8 $SuccessVersionFile
}

if ($Version -eq "latest") {
    $VersionNumber = "15.1.10502.1"
} elseif ($Version -eq "vs2017u1") {
    $VersionNumber = "15.1.10502.1"
}
Write-Host "Info: Using vsdbg version '$VersionNumber'"

if (-not $RuntimeID) {
    $RuntimeID = "win7-x64"
}
Write-Host "Info: Using Runtime ID '$RuntimeID'"

# if we were given a relative path, assume its relative to the script directory and create an absolute path
if (-not([System.IO.Path]::IsPathRooted($InstallPath))) {
    $InstallPath = Join-Path -Path (Split-Path -Path $MyInvocation.MyCommand.Definition) -ChildPath $InstallPath
}

if (IsLatest $InstallPath $RuntimeID $VersionNumber) {
    Write-Host "Info: Latest version of VsDbg is present. Skipping downloads"
} else {
    if ($RemoveExistingOnUpgrade) {
        if (Test-Path $InstallPath) {
            Write-Host "Info: $InstallPath exists, deleting."
            Remove-Item $InstallPath -Force -Recurse -ErrorAction Stop
        }
    }

    $target = ("vsdbg-" + $VersionNumber).Replace('.','-') + "/vsdbg-" + $RuntimeID + ".zip"
    $url = "https://vsdebugger.azureedge.net/" + $target

    if (Test-Path $InstallPath) {
        Remove-Item -Path $InstallPath -Force -Recurse
    }

    DownloadAndExtract $url $InstallPath

    WriteSuccessInfo $InstallPath $RuntimeID $VersionNumber
    Write-Host "Info: Successfully installed vsdbg at '$InstallPath'"
}


