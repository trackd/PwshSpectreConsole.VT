function Get-SpectreGif {
    [CmdletBinding()]
    param(
        [string] $ImagePath,
        [int] $width,
        [int] $LoopCount = 0,
        [Switch] $alt
    )
    $ImagePath = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($ImagePath)
    $imagePathResolved = Resolve-Path $ImagePath
    if (-not (Test-Path $imagePathResolved)) {
        throw "The specified image path '$resolvedImagePath' does not exist."
    }
    $cts = [System.Threading.CancellationTokenSource]::new()
    $player = [PwshSpectreConsole.GifPlayer]::new()
    try {
        if ($alt) {
            $task = $player.PlayAlt($ImagePath, $width, $LoopCount, $cts.Token)
        }
        else {
            $task = $player.Play($ImagePath, $width, $LoopCount, $cts.Token)
        }
        while (-not $task.AsyncWaitHandle.WaitOne(200)) {
            # Waiting for the async task this way allows ctrl-c interrupts to continue to work within the single-threaded PowerShell world
        }
        return $task.GetAwaiter().GetResult() | Out-Null
    }
    finally {
        if ($cts) {
            $cts.Cancel() | Out-Null
        }
        if ($task.IsCompleted) {
            $task.Dispose() | Out-Null
        }
    }
}
