param($action, $webhost)

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

$iis_wwwroot = "www"
$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

# Stop
if ($webhost -eq 'iis') {
	if (Get-WebSite -Name Benchmarks) { Remove-WebSite -Name Benchmarks }
	Get-ChildItem -Path $wwwroot -Recurse -ErrorAction 'SilentlyContinue' | Remove-Item -Force -Recurse -ErrorAction 'SilentlyContinue'; 
	Remove-Item -Force -Recurse $wwwroot -ErrorAction 'SilentlyContinue'
} else {
	Get-Process | Where-Object { $_.Name -ieq "websharper-warp-sqlprovider" } | Stop-Process
}

if ($action -eq 'start') {
    # Clean
	If (Test-Path bin) {
		Remove-Item bin\* -recurse
	}
	If (Test-Path obj) {
		Remove-Item obj\* -recurse
	}
	If (Test-Path paket-files\fsprojects\SQLProvider\bin) {
		Remove-Item paket-files\fsprojects\SQLProvider\bin\* -recurse
	}
	
    # get dependencies
    paket.exe install

    # Build the SQL Provider
    Set-Location -Path paket-files\fsprojects\SQLProvider
    Exec { & .\build.cmd }
    
    # Build the project
    Set-Location -Path ..\..\..\
	
	if ($webhost -eq 'iis') {
		# Create a website in IIS
		New-Item -Path $wwwroot -Type directory | Out-Null
		New-WebSite -Name Benchmarks -Port 8080 -PhysicalPath $wwwroot
		Exec { & $msbuild "websharper-warp-sqlprovider.sln" /p:DeployOnBuild=true /p:PublishProfile=IIS /p:DownloadNuGetExe=true /p:RequireRestoreConsent=false /p:Configuration=Release /t:Rebuild }
	} else {
		Exec { & $msbuild "websharper-warp-sqlprovider.sln" /p:DownloadNuGetExe=true /p:RequireRestoreConsent=false /p:Configuration=Release /t:Rebuild }
		Start-Process "bin\Release\websharper-warp-sqlprovider.exe"
	}
}