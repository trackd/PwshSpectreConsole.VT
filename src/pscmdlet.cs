﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PwshSpectreConsole
{
    [Cmdlet(VerbsData.ConvertTo, "SpectreDecoration")]
    /// <summary>
    /// Converts a VT decorated string to Spectre.Colors
    /// does not support multi-colored strings,
    /// in order to be able to use Spectre.Console.Text
    /// </summary>
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
                WriteObject(lookup, enumerateCollection: true);
                return;
            }
            Hashtable ht = (Hashtable)Transform.Map(lookup);
            ht["String"] = Transform.ToCleanString(String);
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
    ///<summary>
    /// converts a VT decorated string to Spectre Console
    /// default outputs a spectre markdown object
    ///
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
                WriteObject(Transform.ToMarkUp(String, true));
                return;
            }
            WriteObject(Transform.ToMarkUp(String));
        }
    }
    [Cmdlet(VerbsCommunications.Write, "SpectreRender")]
    /// <summary>
    /// writes a renderable object to the pipeline
    /// <summary>
    public sealed class RenderSpectre : PSCmdlet
    {
        [Parameter(Position = 0, ValueFromPipeline = true)]
        public Renderable Renderable { get; set; }

        [Parameter()]
        public String String { get; set; }
        protected override void ProcessRecord()
        {
            if (null != String)
            {
                WriteObject(Transform.Render(new Markup(String)));
                return;
            }
            WriteObject(Transform.Render(Renderable));
            return;
        }
    }
}
