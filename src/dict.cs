using System;
using System.Collections.Generic;
using Spectre.Console;

namespace PwshSpectreConsole
{
    internal static class DecorationDictionary
    {
        internal static bool TryGetValue(int key, out Decoration value)
        {
            if (DecorationDict.TryGetValue(key, out Decoration str))
            {
                value = str;
                return true;
            }
            value = Decoration.None;
            return false;
        }
        internal static Dictionary<int, Decoration> DecorationDict { get; } = new Dictionary<int, Decoration>()
        {
            { 0, Decoration.None },
            { 1, Decoration.Bold },
            { 2, Decoration.Dim },
            { 3, Decoration.Italic },
            { 4, Decoration.Underline },
            { 5, Decoration.SlowBlink },
            { 6, Decoration.RapidBlink },
            { 7, Decoration.Invert },
            { 8, Decoration.Conceal },
            { 9, Decoration.Strikethrough }
        };
    }
    internal static class ConsoleColorDict
    {
        // dictionary for 3/4 bit colors to Spectre.Console.Color
        internal static bool TryGetValue(int key, out Color value)
        {
            if (ColorDict.TryGetValue(key, out Color _color))
            {
                value = _color;
                return true;
            }
            value = Color.Default;
            return false;
        }
        internal static Dictionary<int, Color> ColorDict { get; } = new Dictionary<int, Color>()
        {
            { 0, Color.Black },
            { 1, Color.Navy },
            { 2, Color.Green },
            { 3, Color.Teal },
            { 4, Color.Maroon },
            { 5, Color.Purple },
            { 6, Color.Olive },
            { 7, Color.Silver },
            { 8, Color.Grey },
            { 9, Color.Blue },
            { 10, Color.Lime },
            { 11, Color.Aqua },
            { 12, Color.Red },
            { 13, Color.Fuchsia },
            { 14, Color.Yellow },
            { 15, Color.White },
            { 30, Color.Black },
            { 31, Color.Maroon },
            { 32, Color.Green },
            { 33, Color.Olive },
            { 34, Color.Navy },
            { 35, Color.Purple },
            { 36, Color.Teal },
            { 37, Color.Silver },
            { 40, Color.Black },
            { 41, Color.Maroon },
            { 42, Color.Green },
            { 43, Color.Olive  },
            { 44, Color.Navy },
            { 45, Color.Purple },
            { 46, Color.Teal },
            { 47, Color.Silver },
            { 90, Color.Grey },
            { 91, Color.Red },
            { 92, Color.Lime },
            { 93, Color.Yellow },
            { 94, Color.Blue },
            { 95, Color.Fuchsia },
            { 96, Color.Aqua },
            { 97, Color.White },
            { 100, Color.Grey },
            { 101, Color.Red },
            { 102, Color.Lime },
            { 103, Color.Yellow },
            { 104, Color.Blue },
            { 105, Color.Fuchsia },
            { 106, Color.Aqua },
            { 107, Color.White }
        };
    }
}
