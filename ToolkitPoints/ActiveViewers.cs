using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using Verse;

namespace ToolkitPoints
{
    public static class ActiveViewers
    {
        static readonly int minutesTillInactive = 30;

        public static List<Viewer> FindViewersActiveInChat()
        {
            List<Viewer> activeViewers = new List<Viewer>();

            foreach (Viewer viewer in Viewers.All)
            {
                if (ViewerTracker.ViewerIsBeingTracker(viewer) && ViewerTracker.MinutesSinceLastActive(viewer) < minutesTillInactive)
                {
                    activeViewers.Add(viewer);
                }
            }

            return activeViewers;
        }
    }
}
