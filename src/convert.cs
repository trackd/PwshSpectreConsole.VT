using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Management.Automation.Host;
using System.IO;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PwshSpectreConsole
{
    public partial class Transform
    {
        public static string ToCleanString(string text)
        {
            // 7.3+
            return PSHostUserInterface.GetOutputString(text, false);
        }
        internal static object Map(List<IVT> input)
        {
            Hashtable ht = new Hashtable
                {
                    { "fg", Color.Default },
                    { "bg", Color.Default },
                    { "decoration", Decoration.None },
                };
            foreach (IVT item in input)
            {
                switch (item)
                {
                    case VtCode vt:
                        if (vt.IsForeground)
                        {
                            ht["fg"] = vt.Color;
                        }
                        else
                        {
                            ht["bg"] = vt.Color;
                        }
                        break;
                    case Deco deco:
                        ht["decoration"] = deco.Decoration;
                        break;
                }
            }
            return ht;
        }
        public static string Render(Renderable renderableObject)
        {
            using (var writer = new StringWriter())
            {
                var output = new AnsiConsoleOutput(writer);
                var settings = new AnsiConsoleSettings { Out = output };
                var console = AnsiConsole.Create(settings);
                console.Write(renderableObject);
                return writer.ToString();
            }
        }
        // [GeneratedRegex("(?=\u001b)")]
        // private static partial Regex GenRegex();
        public static object ToMarkUp(string input, bool AsString = false)
        {
            string[] segments = Regex.Split(input, "(?=\x1b)");
            // string[] segment = GenRegex().Split(input, "(?=\x1b)");
            StringBuilder sb = new();
            Hashtable _previous = new();
            foreach (string str in segments)
            {
                if (string.IsNullOrEmpty(str))
                {
                    continue;
                }
                List<IVT> lookup = Parser.Parse(str);
                Hashtable ht = (Hashtable)Map(lookup);
                ht.Add("String", Transform.ToCleanString(str));

                if (string.IsNullOrEmpty((string)ht["String"]))
                {
                    _previous = ht;
                    continue;
                }
                if ((Color)ht["fg"] == Color.Default && _previous["fg"] != null)
                {
                    ht["fg"] = _previous["fg"];
                    _previous["fg"] = null;
                    // _previous.Clear();
                }
                if ((Color)ht["bg"] == Color.Default && _previous["bg"] != null)
                {
                    ht["bg"] = _previous["bg"];
                    _previous["bg"] = null;
                }
                if ((Decoration)ht["decoration"] == Decoration.None && _previous["decoration"] != null)
                {
                    ht["decoration"] = _previous["decoration"];
                    // _previous["decoration"] = null;
                }
                Color fgColor = (Color)ht["fg"];
                Color bgColor = (Color)ht["bg"];
                if ((Decoration)ht["decoration"] != Decoration.None)
                {
                    sb.Append($"[{(Decoration)ht["decoration"]} {fgColor.ToMarkup()} on {bgColor.ToMarkup()}]{(string)ht["String"]}[/]");
                }
                else
                {
                    sb.Append($"[{fgColor.ToMarkup()} on {bgColor.ToMarkup()}]{(string)ht["String"]}[/]");
                }
                _previous.Clear();
            }
            if (AsString)
            {
                return sb.ToString();
            }
            return new Markup(sb.ToString());
        }
    }
}
