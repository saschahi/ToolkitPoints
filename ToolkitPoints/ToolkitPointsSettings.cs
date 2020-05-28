using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
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
            GameFont original = Text.Font;
            Text.Font = GameFont.Medium;

            Rect basePointSettings = new Rect(0, 0, inRect.width - 150f, inRect.height - 200);

            GUI.BeginGroup(basePointSettings);

            Rect pointsBase = new Rect(0, 0, 200, 24);

            Widgets.Label(pointsBase, "Points Base Name:");

            pointsBase.x += pointsBase.width + WidgetRow.LabelGap;

            pointsBaseName = Widgets.TextField(pointsBase, pointsBaseName);

            Rect awardViewers = new Rect(0, pointsBase.y + pointsBase.height, 200f, 24f);

            Widgets.Label(awardViewers, $"Reward Viewers with {pointsBaseName}?");

            awardViewers.x += awardViewers.width + WidgetRow.LabelGap;

            Widgets.Checkbox(awardViewers.position, ref rewardPoints);

            Rect minutesBetween = new Rect(0, awardViewers.y + awardViewers.height, 200f, 24f);

            Widgets.Label(minutesBetween, $"Minutes Between {pointsBaseName} Rewards");

            minutesBetween.x += minutesBetween.width + WidgetRow.LabelGap;

            string minutesBuffer = minutesBetweenRewards.ToString();
            Widgets.TextFieldNumeric(minutesBetween, ref minutesBetweenRewards, ref minutesBuffer);

            Rect pointsPer = new Rect(0, minutesBetween.y + minutesBetween.height, 200f, 24f);

            Widgets.Label(pointsPer, $"{pointsBaseName} Per Reward");

            pointsPer.x += pointsPer.width + WidgetRow.LabelGap;

            string pointsPerBuffer = pointsPerReward.ToString();
            Widgets.TextFieldNumeric(pointsPer, ref pointsPerReward, ref pointsPerBuffer);

            GUI.EndGroup();

            Rect windowButtons = new Rect(basePointSettings.width, 0, 150f, inRect.height);

            GUI.BeginGroup(windowButtons);

            Rect updateAndReward = new Rect(0, 0, 150f, 24f);

            if (Widgets.ButtonText(updateAndReward, "Reward Viewers"))
            {
                Rewarder.TryRewardingViewers();
            }

            Rect activeLedger = new Rect(0, updateAndReward.y + updateAndReward.height, 150f, 24f);

            if (Widgets.ButtonText(activeLedger, "Active Ledger"))
            {
                ManageLedgerWindow window = new ManageLedgerWindow();
                Find.WindowStack.TryRemove(window.GetType());
                Find.WindowStack.Add(window);
            }

            Rect allLedgers = new Rect(0, activeLedger.y + activeLedger.height, 150f, 24f);

            if (Widgets.ButtonText(allLedgers, "All Ledgers"))
            {
                LedgersWindow window = new LedgersWindow();
                Find.WindowStack.TryRemove(window.GetType());
                Find.WindowStack.Add(window);
            }

            GUI.EndGroup();

            Text.Font = original;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref rewardPoints, "rewardPoints", true);
            Scribe_Values.Look(ref pointsBaseName, "pointsBaseName", "Points");
            Scribe_Values.Look(ref minutesBetweenRewards, "minutesBetweenRewards", 5);
            Scribe_Values.Look(ref pointsPerReward, "pointsPerReward", 50);
            Scribe_Values.Look(ref currentActiveLedgerId, "currentActiveLedgerId", -1);

            Scribe_Collections.Look(ref pointLedger, "pointLedger");
            Scribe_Collections.Look(ref lastActive, "lastActive", LookMode.Value, LookMode.Value, ref lastActiveIds, ref lastActiveDateTimes);
        }
    }
}
