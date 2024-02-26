$sb = {
    param($path)
    $b = Join-Path (Split-Path $path -Parent) PwshSpectreConsole
    & "$path\build.ps1"
    Import-Module $b\PwshSpectreConsole\PwshSpectreConsole.psd1 -Force
    $test = '{0}{1}First - {2}{3}Second - {4}{5}{6}Bold' -f $PSStyle.Foreground.Red, $PSStyle.Background.Green, $PSStyle.Foreground.Green, $psstyle.Background.Red, $PSStyle.Blink, $PSStyle.Background.Yellow, $PSStyle.Foreground.BrightCyan
    $sample = "$($PSStyle.Foreground.Red)Hello$($PSStyle.Reset) $($PSStyle.Background.Magenta)$($PSStyle.FormatHyperlink('world!','https://www.example.com'))for sure"
    $sample2 = "$($PSStyle.Foreground.Red)Hello$($PSStyle.Reset) $($PSStyle.Background.Magenta)https://www.example.com$($PSStyle.Reset)"
    filter EscapeAnsi {
        $_.EnumerateRunes() | ForEach-Object {
            if ($_.Value -le 0x1f) {
                [Text.Rune]::new($_.Value + 0x2400)
            }
            else {
                $_
            }
        } | Join-String
    }
    function TestClass {
        param($string)
        Write-Host "String length: $($string.Length)"
        Write-Host "String Escaped:  $($string | escapeansi)"
        [PwshSpectreConsole.Transform]::MultiVT($string) | ForEach-Object {
            [PSCustomObject]@{
                Index          = $_.Index
                Length         = $_.Length
                Text           = $_.Text | EscapeAnsi
                Mapped          = $_.EscapeSequence
            }
        } | Format-SpectreTable -Border Markdown
    }
    function TestMarkup {
        param($string)
            # $foo = [PwshSpectreConsole.Transform]::fromVTToSpectre($string)
            $obj = ConvertTo-SpectreMultiColor $string
            $str = ConvertTo-SpectreMultiColor $string -AsString
            # $foo | Format-SpectreTable -Border Markdown
        [PSCustomObject]@{
            Render   = Write-SpectreRender $obj
            AsString = $str
            Escaped  = $string | EscapeAnsi
        }
    }
    TestClass $test
    TestClass $sample
    TestClass $sample2
    TestMarkup $test
    TestMarkup $sample
    TestMarkup $sample2
}
pwsh -NoProfile -NonInteractive -Command $sb -args $PSScriptRoot

<#
$test = '{0}{1} First - {2}{3}Second - {4}{5}Bold' -f $PSStyle.Foreground.Red, $PSStyle.Background.Green, $PSStyle.Foreground.Green, $psstyle.Background.Red, $PSStyle.Blink, $PSStyle.Background.Yellow

$foo = [PwshSpectreConsole.Transform]::GetTextFragments($test)

#>
