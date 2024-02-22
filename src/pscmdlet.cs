using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Spectre.Console;

namespace PwshSpectreConsole
{
    [Cmdlet(VerbsData.ConvertTo, "SpectreDecoration")]
    [Alias("ToDeco")]
    public sealed class ConvertToSpectreDecorationCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string String { get; set; }
        [Parameter()]
        [Alias("AllowMarkup")]
        public SwitchParameter ToMarkUp { get; set; }
        [Parameter()]
        public SwitchParameter Raw { get; set; }
        [Parameter()]
        public SwitchParameter AsHashtable { get; set; }

        protected override void ProcessRecord()
        {
            List<IVT> lookup = Parser.Parse(String);
            if (Raw)
            {
                WriteObject(lookup);
                return;
            }
            Hashtable ht = (Hashtable)VTConversion.Map(lookup);
            ht["String"] = Clean.ToStripVT(String);
            foreach (var key in MyInvocation.BoundParameters.Keys)
            {
                switch (key)
                {
                    case "AsHashtable":
                        WriteObject(ht);
                        return;
                    case "ToMarkup":
                        Markup _markup = new Markup((string)ht["String"], new Style((Color?)ht["fg"], (Color?)ht["bg"], (Decoration?)ht["decoration"]));
                        WriteObject(_markup);
                        return;
                    default:
                        Text _text = new Text((string)ht["String"], new Style((Color?)ht["fg"], (Color?)ht["bg"], (Decoration?)ht["decoration"]));
                        WriteObject(_text);
                        return;
                }
            }
        }
    }
    [Cmdlet(VerbsData.ConvertTo, "SpectreMarkUp")]
    [Alias("ToMark")]
    public sealed class ConvertToSpectreMarkUpCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string String { get; set; }
        [Parameter()]
        public SwitchParameter AsString { get; set; }
        protected override void ProcessRecord()
        {
            if (AsString)
            {
                WriteObject(VTConversion.ToMarkUp(String, true));
            }
            else
            {
                WriteObject(VTConversion.ToMarkUp(String));
            }
        }
    }
}
