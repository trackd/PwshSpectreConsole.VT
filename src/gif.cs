using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Spectre.Console;
using System.Collections.Generic;

// https://github.com/khalidabuhakmeh/AnimatedGifConsole
namespace PwshSpectreConsole
{
  public class GifPlayer
  {
    public async Task Play(string filePath, int width, int loopCount, CancellationToken cancellationToken)
    {
      await AnsiConsole.Live(Text.Empty)
      .StartAsync(async ctx =>
      {
        using var gif = await Image<Rgba32>.LoadAsync(filePath);
        var metadata = gif.Frames.RootFrame.Metadata.GetGifMetadata();
        var framesData = new List<(int Delay, byte[] Data)>();
        for (int i = 0; i < gif.Frames.Count; i++)
        {
          var delay = gif.Frames[i].Metadata.GetGifMetadata().FrameDelay;
          using var clone = gif.Frames.CloneFrame(i);
          await using var memoryStream = new MemoryStream();
          await clone.SaveAsBmpAsync(memoryStream, cancellationToken);
          memoryStream.Position = 0;
          framesData.Add((Delay: delay, Data: memoryStream.ToArray()));
        }
        int loopCounter = 0;
        while (!cancellationToken.IsCancellationRequested && (loopCount == 0 || loopCounter < loopCount))
        {
          foreach (var frameData in framesData)
          {
            var canvasImage = new CanvasImage(new MemoryStream(frameData.Data)).MaxWidth(width);
            ctx.UpdateTarget(canvasImage);
            await Task.Delay(TimeSpan.FromMilliseconds(frameData.Delay * 5), cancellationToken);
          }
          loopCounter++;
        }
      });
    }
  }
}
