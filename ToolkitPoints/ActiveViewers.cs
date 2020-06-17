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

            foreach (Viewer viewer in ViewerList.Instance.All)
            {
                int minutesSinceLastMessage = DateTime.Now.Subtract(viewer.LastSeen()).Minutes;
                if (minutesSinceLastMessage <= 30)
                {
                    activeViewers.Add(viewer);
                    Log.Message($"Viewer last seen {minutesSinceLastMessage} minutes ago added to active viewer list.");
                }
            }

            return activeViewers;
        }
    }
}
