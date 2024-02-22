using System.Collections.Generic;
using System.Management.Automation;
using Spectre.Console;
using PwshSpectreConsole;

namespace PwshSpectreConsole
{
    [Cmdlet(VerbsData.ConvertTo, "SpectreDecoration")]
    public class ConvertToSpectreDecorationCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string String { get; set; }
        [Parameter]
        public SwitchParameter ToMarkUp { get; set; }

        protected override void ProcessRecord()
        {
            List<IVT> lookup = Parser.Parse(input: String);
            Color fg = default;
            Color bg = default;
            Decoration decoration = default;
            foreach (IVT item in lookup)
            {
                switch (item)
                {
                    case VtCode vt:
                        if (vt.IsForeground)
                        {
                            fg = vt.Color;
                        }
                        else
                        {
                            bg = vt.Color;
                        }
                        break;
                    case Deco deco:
                        decoration = deco.Decoration;
                        break;
                }
            }
            String = Clean.ToStripVT(String);
            if (ToMarkUp)
            {
                Markup _markup = new Markup(String, new Style(fg, bg, decoration));
                WriteObject(_markup);
                return;
            }
            Text _text = new Text(String, new Style(fg, bg, decoration));
            WriteObject(_text);
        }
    }
    [Cmdlet(VerbsData.ConvertTo, "SpectreMarkUp")]
    public class ConvertToSpectreMarkUp : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string String { get; set; }
        protected override void ProcessRecord()
        {
            WriteObject(VTConversion.ToMarkUp(String));
        }
    }
}
