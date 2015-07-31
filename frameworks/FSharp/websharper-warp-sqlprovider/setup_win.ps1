param($action)

$ErrorActionPreference = 'Stop'

# From http://zduck.com/2012/powershell-batch-files-exit-codes/
function Exec
{
    [CmdletBinding()]
    param (
        [Parameter(Position=0, Mandatory=1)]
        [scriptblock]$Command,
        [Parameter(Position=1, Mandatory=0)]
        [string]$ErrorMessage = "Execution of command failed.`n$Command"
    )
    & $Command
    if ($LastExitCode -ne 0) {
        throw "Exec: $ErrorMessage"
    }
}

$root = "C:\FrameworkBenchmarks\websharper-warp-sqlprovider"
$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"

# Stop
Get-Process | Where-Object { $_.Name -ieq "websharper-warp-sqlprovider" } | Stop-Process

if ($action -eq 'start') {
    # Clean
    Remove-Item $root\bin\* -recurse
    Remove-Item $root\obj\* -recurse
    Remove-Item $root\paket-files\fsprojects\SQLProvider\bin\* -recurse

    # get dependencies
    paket.exe install

    # Build the SQL Provider
    Set-Location -Path $root\paket-files\fsprojects\SQLProvider
    Exec { & build.cmd }
    
    # Build the project
    Set-Location -Path $root
    Exec { & $msbuild "$root\websharper-warp-sqlprovider.sln" /p:DownloadNuGetExe=true /p:RequireRestoreConsent=false /p:Configuration=Release /t:Rebuild }
    
    Start-Process "$root\bin\Release\websharper-warp-sqlprovider.exe"
}