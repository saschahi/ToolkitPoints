using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ToolkitPoints
{
    public class ToolkitPointsSettings : ModSettings
    {
        public static bool rewardPoints = true;
        public static string pointsBaseName = "Points";
        public static int minutesBetweenRewards = 5;
        public static int pointsPerReward = 50;

        public static Dictionary<string, int> pointLedger = new Dictionary<string, int>();

        public static Dictionary<string, DateTime> lastActive = new Dictionary<string, DateTime>();

        public List<string> lastActiveIds = new List<string>();
        public List<DateTime> lastActiveDateTimes = new List<DateTime>();

        public void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.ColumnWidth = inRect.width * 0.75f;

            pointsBaseName = listing.TextEntryLabeled("Points Base Name", pointsBaseName);

            listing.CheckboxLabeled($"Reward viewers with {pointsBaseName}", ref rewardPoints, $"Reward viewers {pointsPerReward} {pointsBaseName} every {minutesBetweenRewards} minutes.");

            string minutesBuffer = minutesBetweenRewards.ToString();
            listing.TextFieldNumericLabeled($"Minutes Between {pointsBaseName} Rewards", ref minutesBetweenRewards, ref minutesBuffer, 0);

            string pointsBuffer = pointsPerReward.ToString();
            listing.TextFieldNumericLabeled($"{pointsBaseName} Per Reward", ref pointsPerReward, ref pointsBuffer, 0);

            listing.Label(minutes1);
            listing.Label(minutes2);

            if (listing.ButtonText("Update Chatters"))
            {
                TwitchAPI.UpdateChatters();
            }

            if (listing.ButtonText("Parse Chatters"))
            {
                ChatterParse.ParseChatterString();
            }

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref rewardPoints, "rewardPoints", true);
            Scribe_Values.Look(ref pointsBaseName, "pointsBaseName", "Points");
            Scribe_Values.Look(ref minutesBetweenRewards, "minutesBetweenRewards", 5);
            Scribe_Values.Look(ref pointsPerReward, "pointsPerReward", 50);

            Scribe_Collections.Look(ref pointLedger, "pointLedger");
            Scribe_Collections.Look(ref lastActive, "lastActive", LookMode.Value, LookMode.Value, ref lastActiveIds, ref lastActiveDateTimes);
        }

        // REMOVE
        public static string minutes1 = "";
        public static string minutes2 = "";
    }
}
