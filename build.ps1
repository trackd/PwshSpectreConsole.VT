$old = Get-Location
Set-Location $PSScriptRoot
dotnet clean
dotnet build
Remove-Item '..\PwshSpectreConsole\PwshSpectreConsole\packages\PwshSpectreConsole\PwshSpectreConsole.dll' -Force
Copy-Item '.\bin\Debug\net7.0\PwshSpectreConsole.dll' '..\PwshSpectreConsole\PwshSpectreConsole\packages\PwshSpectreConsole' -Force
Set-Location $old
