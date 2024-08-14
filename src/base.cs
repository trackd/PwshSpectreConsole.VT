using Spectre.Console;
using System.Collections.Generic;

namespace PwshSpectreConsole
{
    public interface IVT
    {
        int Position { get; set; }
    }
    public class BaseVT : IVT
    {
        public int Position { get; set; }
    }
    public class VtCode : BaseVT
    {
        public Color Color { get; set; }
        public bool IsForeground { get; set; }
        public DecoratedType Type { get; set; }
        public VtCode()
        {
            Color = Color.Default;
            Type = DecoratedType.None;
            IsForeground = true;
        }
        public override string ToString()
        {
            return $"{(IsForeground ? "Foreground" : "Background")} {Color.ToMarkup()}";
        }
    }
    public class Deco : BaseVT
    {
        public Decoration Decoration { get; set; }
        // public Deco()
        // {
        //     Decoration = Decoration.None;
        // }
        public override string ToString()
        {
            return $"Decoration({Decoration})";
        }
    }
    public class DecoratedString
    {
        // does it have any vt codes
        public bool IsDecorated { get; set; }
        // original string
        public string? String { get; set; }
        // Text part of the string
        public string? Text { get; set; }
        // length of the original string
        public int? StringLength { get; set; }
        // length of the text. This is the length of the string without vt codes
        public int? TextLength { get; set; }
        // vt codes
        public List<IVT>? VtCodes { get; set; }
    }
    enum DecoratedType
    {
        None
        Color4Bit,
        Color8Bit,
        Color24Bit,
        Decoration
    }
}
