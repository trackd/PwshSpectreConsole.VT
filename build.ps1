param(
    $Type = 'Publish',
    $version = '6.0'
)
Push-Location $PSScriptRoot
Remove-Item '..\PwshSpectreConsole\PwshSpectreConsole\packages\PwshSpectreConsole\PwshSpectreConsole.dll' -Force -ErrorAction Ignore
dotnet clean

if ($Type -eq 'Debug') {
    dotnet build
    # Copy-Item '.\bin\Debug\net6.0\PwshSpectreConsole.dll' '..\PwshSpectreConsole\PwshSpectreConsole\packages\PwshSpectreConsole' -Force
}
else {
    dotnet publish -c Release -f "net$($version)"
    Copy-Item ".\bin\Release\net$($version)\PwshSpectreConsole.dll" '..\PwshSpectreConsole\PwshSpectreConsole\packages\PwshSpectreConsole' -Force
}

Pop-Location
