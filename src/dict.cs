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
    internal static class ConsoleColorLookup
    {
        internal static bool TryGetValue(int key, out string value)
        {
            if (ConsoleColorDict.TryGetValue(key, out string str))
            {
                value = str;
                return true;
            }
            value = null;
            return false;
        }
        internal static Dictionary<int, string> ConsoleColorDict { get; } = new Dictionary<int, string>()
        {
            // ideally this would be a Color type dictionary but Spectre doesnt have all the ConsoleColor names.
            { 30, "black" },
            { 31, "DarkRed" },
            { 32, "DarkGreen" },
            { 33, "DarkYellow" },
            { 34, "DarkBlue" },
            { 35, "DarkMagenta" },
            { 36, "DarkCyan" },
            { 37, "Gray" },
            { 40, "Black" },
            { 41, "DarkRed" },
            { 42, "DarkGreen" },
            { 43, "DarkYellow" },
            { 44, "DarkBlue" },
            { 45, "DarkMagenta" },
            { 46, "DarkCyan" },
            { 47, "Gray" },
            { 90, "DarkGray" },
            { 91, "Red" },
            { 92, "Green" },
            { 93, "Yellow" },
            { 94, "Blue" },
            { 95, "Magenta" },
            { 96, "Cyan" },
            { 97, "White" },
            { 100, "DarkGray" },
            { 101, "Red" },
            { 102, "Green" },
            { 103, "Yellow" },
            { 104, "Blue" },
            { 105, "Magenta" },
            { 106, "Cyan" },
            { 107, "White" }
        };
    }
}
