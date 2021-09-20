using System;
using System.Collections.Generic;
using ToolkitPoints.Windows;
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
        public static int defaultStartingBalance = 150;
        public static bool useMultipleLedgers = false;

        public static Dictionary<string, DateTime> lastActive = new Dictionary<string, DateTime>();
        public static Ledger activeLedger = null;
        public static List<Ledger> allLedgers = null;

        public void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.CheckboxLabeled($"Reward {pointsBaseName} to Viewers", ref rewardPoints);

            pointsBaseName = listing.TextEntryLabeled("Points Base Name", pointsBaseName);

            listing.Label($"Minutes between {pointsBaseName} Rewards");
            listing.IntAdjuster(ref minutesBetweenRewards, 1, 1);


            listing.Label($"{pointsBaseName} rewarded every {minutesBetweenRewards} minutes");

            string pointsPerRewardBuffer = pointsPerReward.ToString();
            listing.IntEntry(ref pointsPerReward, ref pointsPerRewardBuffer);


            listing.Label("Starting balance for new viewers");

            string defaultStartingBalanceBuffer = defaultStartingBalance.ToString();
            listing.IntEntry(ref defaultStartingBalance, ref defaultStartingBalanceBuffer);

            listing.CheckboxLabeled("Using multiple ledgers", ref useMultipleLedgers);

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref rewardPoints, "rewardPoints", true);
            Scribe_Values.Look(ref pointsBaseName, "pointsBaseName", "Points");
            Scribe_Values.Look(ref minutesBetweenRewards, "minutesBetweenRewards", 5);
            Scribe_Values.Look(ref pointsPerReward, "pointsPerReward", 50);
            Scribe_Values.Look(ref defaultStartingBalance, "defaultStartingBalance", 150);
            Scribe_Values.Look(ref useMultipleLedgers, "useMultipleLedgers", false);
        }
    }
}
