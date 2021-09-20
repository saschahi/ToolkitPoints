using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using Verse;

namespace ToolkitPoints
{
    public static class Rewarder
    {
        static List<Viewer> viewers = new List<Viewer>();

        public static async void TryRewardingViewers()
        {
            await TwitchAPI.UpdateChatters();

            List<Viewer> activeChatters = ChatterParse.FindViewersInChatterWrapper();
            List<Viewer> activeViewers = ActiveViewers.FindViewersActiveInChat();

            List<Viewer> viewersToReward = activeChatters.Union(activeViewers).ToList();

            viewers = viewersToReward;

            GiveViewersPoints();
        }

        public static void GiveViewersPoints()
        {
            Log.Message($"Total viewers to reward points: {TCText.ColoredText(viewers.Count.ToString(), ColorLibrary.BlueGreen)}");

            foreach(Viewer viewer in viewers)
            {
                ViewerBalance balance = ToolkitPointsSettings.activeLedger.GetViewerBalance(viewer.Username);

                balance.AddPoints(ToolkitPointsSettings.pointsPerReward);
            }
        }
    }
}
