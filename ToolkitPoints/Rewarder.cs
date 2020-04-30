using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore.Models;
using Verse;

namespace ToolkitPoints
{
    public static class Rewarder
    {
        public static async void TryRewardingViewers()
        {
            Log.Message("Downloading Chatters via Twitch API");

            await TwitchAPI.UpdateChatters();

            Log.Message("Parsing JSON string from Twitch API");

            List<Viewer> activeChatters = ChatterParse.ParseChatterString();
            List<Viewer> activeViewers = ActiveViewers.FindViewersActiveInChat();

            List<Viewer> viewersToReward = activeChatters.Union(activeViewers).ToList();

            Log.Message($"Found {viewersToReward.Count} to reward");
        }
    }
}
