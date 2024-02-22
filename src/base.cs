using Spectre.Console;

namespace PwshSpectreConsole
{
    internal interface IVT
    {
        int Position { get; set; }
#nullable enable
        // string? String { get; set; }
    }

    internal class BaseVT : IVT
    {
        public int Position { get; set; }
        // public string? String { get; set; }
    }
    internal class VtCode : BaseVT
    {
        public Color Color { get; set; }
        public bool IsForeground { get; set; }
        public string Type { get; set; }

        public VtCode()
        {
            Color = Color.Default;
            Type = "Default";
            IsForeground = true;
        }
    }
    internal class Deco : BaseVT
    {
        public Decoration Decoration { get; set; }
        public Deco()
        {
            Decoration = Decoration.None;
        }
    }

}
