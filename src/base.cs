using Spectre.Console;

namespace PwshSpectreConsole
{
    public interface IVT
    {
        int Position { get; set; }
#nullable enable
        string? String { get; set; }
    }

    public class BaseVT : IVT
    {
        public int Position { get; set; }
#nullable enable
        public string? String { get; set; }
    }
    public class VtCode : BaseVT
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
    public class Deco : BaseVT
    {
        public Decoration Decoration { get; set; }
        public Deco()
        {
            Decoration = Decoration.None;
        }
    }

}
