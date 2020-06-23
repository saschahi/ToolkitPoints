using System;
using System.Collections.Generic;
using ToolkitCore.Utilities;
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

        public static int currentActiveLedgerId = -1;

        public static Dictionary<string, int> pointLedger = new Dictionary<string, int>();

        public static Dictionary<string, DateTime> lastActive = new Dictionary<string, DateTime>();

        public List<string> lastActiveIds = new List<string>();
        public List<DateTime> lastActiveDateTimes = new List<DateTime>();

        public void DoWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            GameFont original = Text.Font;
            Text.Font = GameFont.Medium;

            // Button texts
            string rewardButtonText = "Reward Viewers";
            string activeLedgerText = "Active Ledger";
            string allLedgersText = "All Ledgers";

            // Rect regions
            float estBtnWidth = Mathf.Max(
                                    Text.CalcSize(rewardButtonText).x,
                                    Text.CalcSize(activeLedgerText).x,
                                    Text.CalcSize(allLedgersText).x
                                )
                                + 24f;

            Rect buttonRegion = new Rect(inRect.width - estBtnWidth, 0f, estBtnWidth, inRect.height);
            Rect settingsRegion = new Rect(0f, 0f, inRect.width - buttonRegion.width - 23f, inRect.height);

            GUI.BeginGroup(inRect);

            Listing_Standard listing = new Listing_Standard(GameFont.Medium);

            listing.Begin(settingsRegion);
            DrawSettingsPanel(listing);
            listing.End();

            float btnLineHeight = inRect.height * 0.9f;
            Widgets.DrawLineVertical(
                settingsRegion.x + settingsRegion.width + 12f,
                0f,
                btnLineHeight
            );

            listing.Begin(buttonRegion);

            if (listing.ButtonText(rewardButtonText))
            {
                Rewarder.TryRewardingViewers();
            }

            if (listing.ButtonText(activeLedgerText))
            {
                ManageLedgerWindow window = new ManageLedgerWindow();
                Find.WindowStack.TryRemove(window.GetType());
                Find.WindowStack.Add(window);
            }

            if (listing.ButtonText(allLedgersText))
            {
                LedgersWindow window = new LedgersWindow();
                Find.WindowStack.TryRemove(window.GetType());
                Find.WindowStack.Add(window);
            }

            listing.End();
            GUI.EndGroup();

            Text.Font = original;
        }

        private void DrawSettingsPanel(Listing listing)
        {
            (Rect nameLabel, Rect nameField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(nameLabel, "Points Base Name:");
            pointsBaseName = Widgets.TextField(nameField, pointsBaseName);

            if (!pointsBaseName.NullOrEmpty() && SettingsHelper.DrawClearButton(nameField))
            {
                pointsBaseName = "";
            }

            listing.Gap(Text.LineHeight);
            (Rect awardLabel, Rect awardField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(awardLabel, $"Reward viewers with {pointsBaseName}?");
            Widgets.Checkbox(awardField.x + awardField.width - 24f, awardField.y, ref rewardPoints);

            if (!rewardPoints)
            {
                return;
            }

            string minutesBuffer = minutesBetweenRewards.ToString();
            (Rect minutesLabel, Rect minutesField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(minutesLabel, $"Minutes Between {pointsBaseName} Rewards");
            Widgets.TextFieldNumeric(minutesField, ref minutesBetweenRewards, ref minutesBuffer);

            string pointsBuffer = pointsPerReward.ToString();
            (Rect pointsLabel, Rect pointsField) = listing.GetRect(Text.LineHeight).ToForm();
            Widgets.Label(pointsLabel, $"{pointsBaseName} Per Reward");
            Widgets.TextFieldNumeric(pointsField, ref pointsPerReward, ref pointsBuffer);
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref rewardPoints, "rewardPoints", true);
            Scribe_Values.Look(ref pointsBaseName, "pointsBaseName", "Points");
            Scribe_Values.Look(ref minutesBetweenRewards, "minutesBetweenRewards", 5);
            Scribe_Values.Look(ref pointsPerReward, "pointsPerReward", 50);
            Scribe_Values.Look(ref currentActiveLedgerId, "currentActiveLedgerId", -1);

            Scribe_Collections.Look(ref pointLedger, "pointLedger");
            Scribe_Collections.Look(
                ref lastActive,
                "lastActive",
                LookMode.Value,
                LookMode.Value,
                ref lastActiveIds,
                ref lastActiveDateTimes
            );
        }
    }
}
