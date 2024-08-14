using System;
using System.Collections.Generic;
using System.Text;
using Spectre.Console;

namespace PwshSpectreConsole
{
    public class Decoder
    {
        private static VtCode New4BitVT(int firstCode, int _position)
        {
            Color color = new Color();
            if (ConsoleColorDict.TryGetValue(firstCode, out Color _color))
            {
                color = _color;
            }
            return new VtCode
            {
                Color = color,
                Type = DecoratedType.Color4Bit,
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
                Type = DecoratedType.Color8Bit,
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
                Type = DecoratedType.Color24Bit,
                IsForeground = IsForeground,
                Position = _position
            };
        }
        private static Deco? NewDecoVT(int decorationNumber, int _position)
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
                return null;
            }
        }
        private static IVT? NewVT(int firstCode, byte[] codeParts, int _position)
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
        public static DecoratedString Parse(string input)
        {
            try
            {
                ReadOnlySpan<char> inputSpan = input.AsSpan();
                List<IVT> vtCodes = new List<IVT>();
                StringBuilder textBuilder = new StringBuilder();
                while (!inputSpan.IsEmpty)
                {
                    GetNextSlice(ref inputSpan, out ReadOnlySpan<char> vtCode, out int _position);
                    textBuilder.Append(inputSpan.Slice(0, _position).ToString()); // Append text before VT code
                    if (vtCode == null)
                    {
                        break;
                    }
                    inputSpan = inputSpan.Slice(_position); // Adjust slicing here
                    ReadOnlySpan<char> sliceSpan = vtCode;
                    List<byte> codeParts = new List<byte>();
                    while (sliceSpan.Length > 0)
                    {
                        int separatorIndex = sliceSpan.IndexOf(';');
                        if (separatorIndex == -1)
                        {
                            separatorIndex = sliceSpan.Length;
                        }
                        ReadOnlySpan<char> partSpan = sliceSpan.Slice(0, separatorIndex);
                        if (byte.TryParse(partSpan, out byte code))
                        {
                            codeParts.Add(code);
                        }
                        sliceSpan = sliceSpan.Slice(Math.Min(separatorIndex + 1, sliceSpan.Length));
                    }
                    if (codeParts.Count > 0)
                    {
                        byte firstCode = codeParts[0];
                        IVT? _vtCode = NewVT(firstCode, codeParts.ToArray(), _position);
                        if (_vtCode != null)
                        {
                            vtCodes.Add(_vtCode);
                        }
                    }
                    inputSpan = inputSpan.Slice(vtCode.Length + 3); // Skip over the VT code and the 'm' character
                }
                string text = textBuilder.ToString();
                return new DecoratedString
                {
                    IsDecorated = vtCodes.Count > 0,
                    String = input,
                    Text = text,
                    StringLength = input.Length,
                    TextLength = text.Length,
                    VtCodes = vtCodes
                };
            }
            catch (Exception ex)
            {
                // Add logging or output error details for debugging
                Console.WriteLine($"Error occurred during parsing: {ex.Message}");
                throw; // Re-throw the exception to propagate it further
            }
        }
        private static void GetNextSlice(ref ReadOnlySpan<char> inputSpan, out ReadOnlySpan<char> vtCode, out int _position)
        {
            try
            {
                var escIndex = inputSpan.IndexOf('\u001b');
                if (escIndex == -1 || escIndex + 2 >= inputSpan.Length)
                {
                    vtCode = null;
                    _position = escIndex == -1 ? inputSpan.Length : escIndex;
                    return;
                }
                // Skip the '[' character after ESC
                var sliceStart = escIndex + 2;
                var endIndex = inputSpan.Slice(sliceStart).IndexOf('m');
                if (endIndex == -1 || sliceStart + endIndex >= inputSpan.Length)
                {
                    vtCode = null;
                    _position = sliceStart - 2; // Return position of '\u001b'
                    return;
                }
                vtCode = inputSpan.Slice(sliceStart, endIndex);
                _position = escIndex; // Return position of '\u001b'
                inputSpan = inputSpan.Slice(_position + endIndex + 3); // Include the 'm' character
            }
            catch (Exception ex)
            {
                // Add logging or output error details for debugging
                Console.WriteLine($"Error occurred during VT code extraction: {ex.Message}");
                throw; // Re-throw the exception to propagate it further
            }
        }
    }
}
