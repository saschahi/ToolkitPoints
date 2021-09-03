using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore.Controllers;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using UnityEngine;
using Verse;
using static ToolkitCore.Models.Services;

namespace ToolkitPoints.Windows
{
    public enum SortModes { Ascending, Descending }

    public enum Sorters { Name, Points }

    public class ManageLedgerWindow : Window
    {
        public ManageLedgerWindow(Ledger ledger = null)
        {
            this.doCloseButton = true;
            if (ledger == null)
            {
                activeLedger = Ledgers.ActiveLedger();
            }
            else
            {
                activeLedger = ledger;
            }

            if (activeLedger.LedgerRecords != null && activeLedger.LedgerRecords.Count > 0)
            {
                LedgerRecord selectedViewer = activeLedger.LedgerRecords.FirstOrDefault();
                selectedViewerName = selectedViewer.Username;
                selectedViewerPoints = selectedViewer.PointBalance;
            }
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();
            UpdateSelectedViewerProperties();

            if (lastCachedSearch.Equals(viewerSearch))
            {
                return;
            }

            if (Time.time % 2 < 1)
            {
                Notify_SearchRequested();
            }
        }

        private void Notify_SearchRequested()
        {
            searchResults = GetSearchResults();
            lastCachedSearch = viewerSearch;
        }

        private List<string> GetSearchResults()
        {
            if (viewerSearch.NullOrEmpty())
            {
                return new List<string>();
            }

            return ViewerList.Instance.All
                .Where(
                    v => v.Username.Equals(viewerSearch, StringComparison.InvariantCultureIgnoreCase)
                         || v.Username.ToUpper().Contains(viewerSearch.ToUpper())
                )
                .Take(3)
                .Select(v => v.Username)
                .ToList();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GameFont original = Text.Font;
            Text.Font = GameFont.Small;

            Rect firstColumn = new Rect(0, 0, inRect.width / 2f, inRect.height);
            Rect secondColumn = new Rect(firstColumn.width, 0, firstColumn.width, inRect.height);

            // First Column

            GUI.BeginGroup(firstColumn);

            if (selectedViewerName != null)
            {
                Rect viewerName = new Rect(0, 0, 200f, 24f);

                Widgets.Label(viewerName, "Viewer:");

                viewerName.x += viewerName.width + WidgetRow.LabelGap;

                Widgets.Label(viewerName, selectedViewerName);


                Rect viewerPoints = new Rect(0, 28f, 200f, 24f);

                Widgets.Label(viewerPoints, $"{ToolkitPointsSettings.pointsBaseName}:");

                viewerPoints.x += viewerName.width + WidgetRow.LabelGap;

                string selectedViewerPointsBuffer = selectedViewerPoints.ToString();
                Widgets.TextFieldNumeric(viewerPoints, ref selectedViewerPoints, ref selectedViewerPointsBuffer);


                Rect rewardPoints = new Rect(0, viewerPoints.y + 28f, 200f, 24f);

                Widgets.Label(rewardPoints, $"Give Viewer {ToolkitPointsSettings.pointsBaseName}:");

                rewardPoints.x += rewardPoints.width + WidgetRow.LabelGap;
                rewardPoints.width = 196f + WidgetRow.LabelGap;

                string pointsToRewardViewerBuffer = pointsToRewardViewer.ToString();
                Widgets.TextFieldNumeric(rewardPoints, ref pointsToRewardViewer, ref pointsToRewardViewerBuffer);

                if (SettingsHelper.DrawDoneButton(rewardPoints))
                {
                    selectedViewerPoints += pointsToRewardViewer;
                }


                Rect takePoints = new Rect(0, rewardPoints.y + 28f, 200f, 24f);

                Widgets.Label(takePoints, $"Take Viewer {ToolkitPointsSettings.pointsBaseName}:");

                takePoints.x += takePoints.width + WidgetRow.LabelGap;
                takePoints.width = 196f + WidgetRow.LabelGap;

                string pointsToTakeFromViewerBuffer = pointsToTakeFromViewer.ToString();
                Widgets.TextFieldNumeric(takePoints, ref pointsToTakeFromViewer, ref pointsToTakeFromViewerBuffer);

                if (SettingsHelper.DrawDoneButton(takePoints))
                {
                    selectedViewerPoints -= pointsToTakeFromViewer;
                }
            }

            GUI.EndGroup();

            // Second Column

            GUI.BeginGroup(secondColumn);

            Rect searchBox = new Rect(0, 0, 200f, 24f);

            Widgets.Label(searchBox, "Search:");

            searchBox.x += searchBox.width + WidgetRow.LabelGap;

            viewerSearch = Widgets.TextField(searchBox, viewerSearch);

            if (!viewerSearch.NullOrEmpty() && SettingsHelper.DrawClearButton(searchBox))
            {
                viewerSearch = "";
            }

            if (viewerSearch.NullOrEmpty())
            {
                lastCachedSearch = "";
                
                if(searchResults.Count > 0)
                {
                    searchResults = new List<string>();
                }
            }

            Rect searchResultButtons = new Rect(204f, searchBox.y + searchBox.height, 200f, 24f);

            foreach (string username in searchResults)
            {
                Widgets.DrawAtlas(searchResultButtons, TexUI.FloatMenuOptionBG);
                
                if (Widgets.ButtonText(searchResultButtons, $" {username}", false))
                {
                    UpdateSelectedViewer(username);
                }

                searchResultButtons.y += searchResultButtons.height;
            }

            bool ledgerRecordsNull = activeLedger.LedgerRecords == null;
            int pointKeys = ledgerRecordsNull ? 0 : activeLedger.LedgerRecords.Count;

            Rect outRect = new Rect(0, 96, secondColumn.width, secondColumn.height - 150f);
            Rect scrollView = new Rect(0, 0, outRect.width - 20f, (24f * pointKeys) + 16);

            Widgets.DrawMenuSection(outRect);

            Widgets.BeginScrollView(outRect, ref scrollPosition, scrollView);

            Rect row = new Rect(10, 10, scrollView.width - 20f, 24f);

            if (!ledgerRecordsNull)
            {
                foreach (LedgerRecord record in activeLedger.LedgerRecords)
                {
                    DoLedgerRow(row, record);
                    row.y += row.height;
                }
            }

            Widgets.EndScrollView();

            GUI.EndGroup();

            Text.Font = original;
        }

        private void DoLedgerRow(Rect row, LedgerRecord ledgerRecord)
        {
            GUI.BeginGroup(row);

            Rect label = new Rect(0, 0, 300f, 24f);
            Rect pointLabel = new Rect(304f, 0, 200f, 24f);

            Viewer viewer = ViewerController.GetViewer(Service.Twitch, ledgerRecord.Username);

            string viewerLabel = TCText.ColoredText(ledgerRecord.Username, ColorLibrary.Gold);

            if (Widgets.ButtonText(label, viewerLabel, false))
            {
                UpdateSelectedViewer(ledgerRecord.Username);
            }

            Widgets.Label(pointLabel, $"{ledgerRecord.PointBalance} {ToolkitPointsSettings.pointsBaseName}");

            GUI.EndGroup();
        }


        private void UpdateSelectedViewer(string username)
        {
            if (activeLedger.LedgerRecords == null)
            {
                activeLedger.LedgerRecords = new List<LedgerRecord>();
            }

            selectedViewerName = username;
            selectedViewerPoints = activeLedger.GetLedgerRecord(username, Service.Twitch).PointBalance;
            selectedViewerPointsCached = selectedViewerPoints;

            viewerSearch = "";
            lastCachedSearch = "";
            searchResults = new List<string>();
        }

        private void UpdateSelectedViewerProperties()
        {
            if (selectedViewerPoints != selectedViewerPointsCached)
            {
                if (selectedViewerPointsCached == int.MinValue)
                {
                    selectedViewerPointsCached = selectedViewerPoints;
                }
                else
                {
                    activeLedger.GetLedgerRecord(selectedViewerName, Service.Twitch).PointBalance = selectedViewerPoints;
                    selectedViewerPointsCached = selectedViewerPoints;
                }
            }
        }

        Vector2 scrollPosition;

        public override Vector2 InitialSize => new Vector2(1000, 600);

        Ledger activeLedger = null;
        string selectedViewerName = null;

        string viewerSearch = "";
        string lastCachedSearch = "";

        int selectedViewerPoints = int.MinValue;
        int selectedViewerPointsCached = int.MinValue;

        int pointsToRewardViewer = 100;
        int pointsToTakeFromViewer = 0;

        private SortModes sortMode;
        private Sorters sorter;

        List<string> searchResults = new List<string>();
    }
}
