using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Management.Automation.Host;
using Spectre.Console;

namespace PwshSpectreConsole
{
    public class Clean
    {
        public static string ToStripVT(string text)
        {
            // 7.3+
            return PSHostUserInterface.GetOutputString(text, false);
        }
    }
    public class VTConversion
    {
        public static Markup ToMarkUp(string input)
        {
            string[] segment = Regex.Split(input, "(?=\x1b)");
            StringBuilder sb = new StringBuilder();
            Hashtable _previous = new Hashtable();
            foreach (string obj in segment)
            {
                if (string.IsNullOrEmpty(obj))
                {
                    continue;
                }
                Hashtable ht = new Hashtable
                {
                    { "fg", Color.Default },
                    { "bg", Color.Default },
                    { "decoration", Decoration.None },
                    { "String", Clean.ToStripVT(obj) }
                };
                List<IVT> lookup = Parser.Parse(obj);
                foreach (IVT item in lookup)
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
                    _previous["decoration"] = null;
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
            }
            return new Markup(sb.ToString());
        }

    }

}
