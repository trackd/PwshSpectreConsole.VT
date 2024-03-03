$writer = [System.IO.StringWriter]::new()
$settings = [Spectre.Console.AnsiConsoleSettings]::new()
$settings.Out = [Spectre.Console.AnsiConsoleOutput]::new($writer)
$console = [Spectre.Console.AnsiConsole]::Create($settings)
$asciiCastOut = [Generator.Commands.AsciiCastConsole]::new($console.Output)
$sample = [Generator.Commands.Samples.LayoutSample]::new()
$sample.Run($console)


$writer = [System.IO.StringWriter]::new()
$output = [Spectre.Console.AnsiConsoleOutput]::new($writer)
$asciiCastOut = [Generator.Commands.AsciiCastOut]::new($output)
$writer = $asciiCastOut.Writer
$writer.Write("Hello, world!")
$json = $asciiCastOut.GetCastJson("My Title")
Write-Host $json


$writer = [System.IO.StringWriter]::new()
$settings = [Spectre.Console.AnsiConsoleSettings]::new()
$settings.Out = [Spectre.Console.AnsiConsoleOutput]::new($writer)
$console = [Spectre.Console.AnsiConsole]::Create($settings)
$asciiCastOut = [Generator.Commands.AsciiCastConsole]::new($console.Output)
$sample = [Generator.Commands.Samples.TextPathSample]::new()
$sample.Run($console)


function test-samples {
    param(
        [string]$class
    )
    [PwshSpectreConsole.SampleRunner]::new().RunSample($class)
    # [PwshSpectreConsole.Commands.Samples.BarChartSample]::new().Run()
}
