using System;
using System.IO;
using Spectre.Console;
using PwshSpectreConsole.Commands;
using PwshSpectreConsole.Commands.Samples;

namespace PwshSpectreConsole
{
  public class SampleRunner
  {
    public void RunSample(string className)
    {
      var writer = new StringWriter();
      var settings = new AnsiConsoleSettings
      {
        Out = new AnsiConsoleOutput(writer)
      };

      var console = AnsiConsole.Create(settings);

      var type = Type.GetType($"PwshSpectreConsole.Commands.Samples.{className}");
      if (type == null)
      {
        throw new Exception($"Failed to find type 'PwshSpectreConsole.Commands.Samples.{className}'");
      }

      dynamic sample = Activator.CreateInstance(type);
      sample.Run(console);

      Console.WriteLine(writer.ToString());
    }
  }
}
