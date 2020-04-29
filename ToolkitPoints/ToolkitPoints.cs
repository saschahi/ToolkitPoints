using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ToolkitPoints
{
    public class ToolkitPoints : Mod
    {
        public ToolkitPoints(ModContentPack content) : base(content)
        {

        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            GetSettings<ToolkitPointsSettings>().DoWindowContents(inRect);
        }
    }
}
