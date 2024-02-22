using System;
using System.Collections.Generic;
using Spectre.Console;
using System.Linq;

namespace PwshSpectreConsole
{
    internal class Parser
    {
        private static (string slice, string nonVtText, int placement) GetNextSlice(ref ReadOnlySpan<char> inputSpan)
        {
            var escIndex = inputSpan.IndexOf('\x1B');
            if (escIndex == -1)
            {
                return (null, inputSpan.ToString(), 0);
            }
            // Skip the '[' character after ESC
            var sliceStart = escIndex + 2;
            if (sliceStart >= inputSpan.Length)
            {
                return (null, inputSpan.ToString(), 0);
            }
            var slice = inputSpan.Slice(sliceStart);
            var endIndex = slice.IndexOf('m');
            if (endIndex == -1)
            {
                return (null, inputSpan.ToString(), 0);
            }
            var vtCode = slice.Slice(0, endIndex).ToString();
            var nonVtText = inputSpan.Slice(0, escIndex).ToString();
            var placement = sliceStart + endIndex - vtCode.Length;
            inputSpan = inputSpan.Slice(placement);
            return (vtCode, nonVtText, placement);
        }
        private static (string slice, int placement) OldGetNextSlice(ref ReadOnlySpan<char> inputSpan)
        {
            var escIndex = inputSpan.IndexOf('\x1B');
            if (escIndex == -1)
            {
                return (null, 0);
            }
            // Skip the '[' character after ESC
            var sliceStart = escIndex + 2;
            if (sliceStart >= inputSpan.Length)
            {
                return (null, 0);
            }
            var slice = inputSpan.Slice(sliceStart);
            var endIndex = slice.IndexOf('m');
            if (endIndex == -1)
            {
                return (null, 0);
            }
            var vtCode = slice.Slice(0, endIndex).ToString();
            var placement = sliceStart + endIndex - vtCode.Length;
            inputSpan = inputSpan.Slice(placement);
            return (vtCode, placement);
        }
        private static VtCode New4BitVT(int firstCode, int _position)
        {
            Color color = new Color();
            if (firstCode > 0 && firstCode < 15)
            {
                color = Color.FromConsoleColor((ConsoleColor)firstCode);
            }
            else
            {
                if (ConsoleColorLookup.TryGetValue(firstCode, out string consoleColor) && Enum.TryParse(consoleColor, out ConsoleColor parsedColor))
                {
                    color = Color.FromConsoleColor(parsedColor);
                }
            }

            return new VtCode
            {
                Color = color,
                Type = "4bit",
                IsForeground = (firstCode >= 30 && firstCode <= 37) || (firstCode >= 90 && firstCode <= 97),
                Position = _position
            };
        }
        private static VtCode New8BitVT(byte[] codeParts, int _position, bool IsForeground)
        {
            Color color = Color.FromInt32(codeParts[2]);
            return new VtCode
            {
                Color = color,
                Type = "8bit",
                IsForeground = IsForeground,
                Position = _position
            };
        }
        private static VtCode New24BitVT(byte[] codeParts, int _position, bool IsForeground)
        {
            Color color = new Color(codeParts[2], codeParts[3], codeParts[4]);

            return new VtCode
            {
                Color = color,
                Type = "24bit",
                IsForeground = IsForeground,
                Position = _position
            };
        }
        private static Deco NewDecoVT(int decorationNumber, int _position)
        {
            if (DecorationDictionary.TryGetValue(decorationNumber, out Decoration decoration))
            {
                return new Deco
                {
                    Decoration = decoration,
                    Position = _position
                };
            }
            else
            {
                return null;  // Or however you want to handle the case where the decoration number is not found
            }
        }
        private static IVT NewVT(int firstCode, byte[] codeParts, int _position)
        {
            if (firstCode >= 30 && firstCode <= 37 || firstCode >= 40 && firstCode <= 47 || firstCode >= 90 && firstCode <= 97 || firstCode >= 100 && firstCode <= 107)
            {
                return New4BitVT(firstCode, _position);
            }
            else if (firstCode == 38 || firstCode == 48)
            {
                bool IsForeground = firstCode == 48 ? false : true;
                if (codeParts.Length >= 3 && codeParts[1] == 5)
                {
                    return New8BitVT(codeParts, _position, IsForeground);
                }
                else if (codeParts.Length >= 5 && codeParts[1] == 2)
                {
                    return New24BitVT(codeParts, _position, IsForeground);
                }
            }
            else
            {
                return NewDecoVT(firstCode, _position);
            }
            return null;
        }
        internal static List<IVT> Parse(string input)
        {
            ReadOnlySpan<char> inputSpan = input.AsSpan();
            List<IVT> results = new List<IVT>();

            while (!inputSpan.IsEmpty)
            {
                // var (slice, placement) = GetNextSlice(inputSpan: ref inputSpan);
                var (slice, nonVtText, placement) = GetNextSlice(ref inputSpan);
                if (slice == null)
                {
                    break;
                }
                byte[] codeParts = slice.Split(';').Select(byte.Parse).ToArray();
                if (codeParts.Length > 0)
                {
                    try
                    {
                        int firstCode = codeParts[0];
                        IVT _vtCode = NewVT(firstCode, codeParts, placement);
                        if (_vtCode != null)
                        {
                            _vtCode.String = nonVtText;
                            results.Add(_vtCode);
                        }
                    }
                    catch (FormatException)
                    {
                        // Ignore
                    }
                }
            }
            return results;
        }
    }
}
