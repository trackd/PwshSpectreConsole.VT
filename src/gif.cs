using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Gif;

// https://github.com/khalidabuhakmeh/AnimatedGifConsole
namespace PwshSpectreConsole
{
  public class PixelDoubler : IRenderable
  {
    private readonly Image<Rgba32> _image;
    private readonly Rgba32 _backgroundColor;

    public PixelDoubler(Image<Rgba32> image)
    {
      _image = image;
      var systemDrawingColor = System.Drawing.Color.FromName(Console.BackgroundColor.ToString());
      _backgroundColor = new Rgba32(systemDrawingColor.R, systemDrawingColor.G, systemDrawingColor.B);
    }

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
      return new Measurement(_image.Width, _image.Width);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
      var line = new StringBuilder();

      for (int y = 0; y < _image.Height; y += 2)
      {
        line.Clear();

        for (int x = 0; x < _image.Width; x++)
        {
          var currentPixel = _image[x, y];
          var foregroundMultiplier = currentPixel.A / 255f;
          var backgroundMultiplier = 1 - foregroundMultiplier;

          var currentPixelRgb = new Rgba32(
              (byte)Math.Min(255, currentPixel.R * foregroundMultiplier + _backgroundColor.R * backgroundMultiplier),
              (byte)Math.Min(255, currentPixel.G * foregroundMultiplier + _backgroundColor.G * backgroundMultiplier),
              (byte)Math.Min(255, currentPixel.B * foregroundMultiplier + _backgroundColor.B * backgroundMultiplier),
              currentPixel.A
          );

          if (y < _image.Height - 1)
          {
            var pixelBelow = _image[x, y + 1];
            foregroundMultiplier = pixelBelow.A / 255f;
            backgroundMultiplier = 1 - foregroundMultiplier;

            var pixelBelowRgb = new Rgba32(
                (byte)Math.Min(255, pixelBelow.R * foregroundMultiplier + _backgroundColor.R * backgroundMultiplier),
                (byte)Math.Min(255, pixelBelow.G * foregroundMultiplier + _backgroundColor.G * backgroundMultiplier),
                (byte)Math.Min(255, pixelBelow.B * foregroundMultiplier + _backgroundColor.B * backgroundMultiplier),
                pixelBelow.A
            );

            line.Append($"\u001b[48;2;{currentPixelRgb.R};{currentPixelRgb.G};{currentPixelRgb.B}m\u001b[38;2;{pixelBelowRgb.R};{pixelBelowRgb.G};{pixelBelowRgb.B}m▄\u001b[0m");
          }
          else
          {
            line.Append($"\u001b[48;2;{currentPixelRgb.R};{currentPixelRgb.G};{currentPixelRgb.B}m\u001b[38;2;{currentPixelRgb.R};{currentPixelRgb.G};{currentPixelRgb.B}m▄\u001b[0m");
          }
        }
        yield return new Segment(line.ToString());
      }
    }
  }
  public class GifPlayer
  {
    public async Task PlayAlt(string filePath, int width, int loopCount, CancellationToken cancellationToken)
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
          using var frame = gif.Frames.CloneFrame(i);
          await using var memoryStream = new MemoryStream();
          await frame.SaveAsBmpAsync(memoryStream, cancellationToken);
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
            await Task.Delay(TimeSpan.FromMilliseconds(frameData.Delay * 10), cancellationToken);
          }
          loopCounter++;
        }
      });
    }
    public async Task Play(string filePath, int width, int loopCount, CancellationToken cancellationToken)
    {
      await AnsiConsole.Live(Text.Empty)
      .StartAsync(async ctx =>
      {
        using var gif = await Image<Rgba32>.LoadAsync(filePath);
        var metadata = gif.Frames.RootFrame.Metadata.GetGifMetadata();
        var framesData = new List<(int Delay, PixelDoubler Frame)>();
        for (int i = 0; i < gif.Frames.Count; i++)
        {
          var pixelDoubler = new PixelDoubler((Image<Rgba32>)gif.Frames[i]);
          framesData.Add((Delay: gif.Frames[i].Metadata.GetGifMetadata().FrameDelay, Frame: pixelDoubler));
        }
        int loopCounter = 0;
        while (!cancellationToken.IsCancellationRequested && (loopCount == 0 || loopCounter < loopCount))
        {
          foreach (var frameData in framesData)
          {
            ctx.UpdateTarget(frameData.Frame);
            await Task.Delay(TimeSpan.FromMilliseconds(frameData.Delay * 10), cancellationToken);
          }
          loopCounter++;
        }
      });
    }
    // public async Task Play3(string filePath, int width, int loopCount, CancellationToken cancellationToken)
    // {
    //   using var gif = await Image.LoadAsync<Rgba32>(filePath);
    //   var framesData = new List<(int Delay, Image<Rgba32> Frame)>();

    //   for (int i = 0; i < gif.Frames.Count; i++)
    //   {
    //     var delay = gif.Frames[i].Metadata.GetFormatMetadata(GifFormat.Instance).FrameDelay;
    //     var frame = gif.Frames.CloneFrame(i);
    //     frame.Mutate(x => x.Resize(width, 0));
    //     framesData.Add((Delay: delay, Frame: frame));
    //   }

    //   int loopCounter = 0;
    //   await AnsiConsole.Live(Text.Empty).StartAsync(async ctx =>
    //   {
    //     while (!cancellationToken.IsCancellationRequested && (loopCount == 0 || loopCounter < loopCount))
    //     {
    //       foreach (var frameData in framesData)
    //       {
    //         var consoleImage = new ConsoleImage(frameData.Frame);
    //         var imageText = new Text(GetImageText(frameData.Frame));
    //         ctx.UpdateTarget(imageText);

    //         await Task.Delay(TimeSpan.FromMilliseconds(frameData.Delay * 10), cancellationToken);
    //       }
    //       loopCounter++;
    //     }
    //   });
    // }

    // private string GetImageText(Image<Rgba32> image)
    // {
    //   var imageText = new StringBuilder();

    //   for (int y = 0; y < image.Height; y += 2)
    //   {
    //     for (int x = 0; x < image.Width; x++)
    //     {
    //       var upperPixel = image[x, y];
    //       var lowerPixel = y < image.Height - 1 ? image[x, y + 1] : upperPixel;

    //       imageText.Append($"[bg #{upperPixel.R:X2}{upperPixel.G:X2}{upperPixel.B:X2}][#{lowerPixel.R:X2}{lowerPixel.G:X2}{lowerPixel.B:X2}]▄[/]");
    //     }
    //     imageText.AppendLine();
    //   }

    //   return imageText.ToString();
    // }
    public async Task Playbb(string filePath, int loopCount, CancellationToken cancellationToken)
    {
      await AnsiConsole.Live(Text.Empty)
      .StartAsync(async ctx =>
      {
        using var gif = await Image<Rgba32>.LoadAsync(filePath);
        var metadata = gif.Frames.RootFrame.Metadata.GetGifMetadata();
        var framesData = new List<(int Delay, CanvasImage Image)>();
        for (int i = 0; i < gif.Frames.Count; i++)
        {
          var delay = gif.Frames[i].Metadata.GetGifMetadata().FrameDelay;
          using var frame = gif.Frames.CloneFrame(i);
          await using var memoryStream = new MemoryStream();
          await frame.SaveAsBmpAsync(memoryStream, cancellationToken);
          memoryStream.Position = 0;
          var canvasImage = new CanvasImage(new MemoryStream(memoryStream.ToArray()));
          // canvasImage.MaxWidth = width;
          canvasImage.PixelWidth = 1;
          canvasImage.BicubicResampler();
          framesData.Add((Delay: delay, Image: canvasImage));
        }
        int loopCounter = 0;
        while (!cancellationToken.IsCancellationRequested && (loopCount == 0 || loopCounter < loopCount))
        {
          foreach (var frameData in framesData)
          {
            ctx.UpdateTarget(frameData.Image);
            await Task.Delay(TimeSpan.FromMilliseconds(frameData.Delay * 10), cancellationToken);
          }
          loopCounter++;
        }
      });
    }
  }
}
