﻿using Spectre.Console;

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
        public string Type { get; set; }
        public VtCode()
        {
            Color = Color.Default;
            Type = "Default";
            IsForeground = true;
        }
        public override string ToString()
        {
            if (IsForeground)
            {
                return $"RGB({Color.R},{Color.G},{Color.B}) Foreground";
            }
            return $"RGB({Color.R},{Color.G},{Color.B}) Background";
        }
    }
    public class Deco : BaseVT
    {
        public Decoration Decoration { get; set; }
        public Deco()
        {
            Decoration = Decoration.None;
        }
        public override string ToString()
        {
            return $"Decoration({Decoration})";
        }
    }

}
