param(
    $Type = 'Publish'
)
$old = Get-Location
Set-Location $PSScriptRoot
Remove-Item '..\PwshSpectreConsole\PwshSpectreConsole\packages\PwshSpectreConsole\PwshSpectreConsole.dll' -Force -ErrorAction Ignore
dotnet clean

if ($Type -eq 'Debug') {
    dotnet build
    Copy-Item '.\bin\Debug\net7.0\PwshSpectreConsole.dll' '..\PwshSpectreConsole\PwshSpectreConsole\packages\PwshSpectreConsole' -Force
}
else {
    dotnet publish -c Release -f net7.0
    Copy-Item '.\bin\Release\net7.0\PwshSpectreConsole.dll' '..\PwshSpectreConsole\PwshSpectreConsole\packages\PwshSpectreConsole' -Force
}

Set-Location $old
