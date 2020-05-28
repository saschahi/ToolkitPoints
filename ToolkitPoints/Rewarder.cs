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

            if (Ledgers.Instance == null)
            {
                Log.Error("ledger instance null");
                return;
            }

            Ledger activeLedger = Ledgers.ActiveLedger();

            if (activeLedger == null)
            {
                Log.Error("Current Active Ledger is null. Report to mod author.");
                return;
            }

            foreach(Viewer viewer in viewers)
            {
                if (activeLedger.Points.ContainsKey(viewer.Username))
                {
                    activeLedger.Points[viewer.Username] += ToolkitPointsSettings.pointsPerReward;
                }
                else
                {
                    activeLedger.Points.Add(viewer.Username, ToolkitPointsSettings.pointsPerReward);
                }
            }
        }
    }
}
