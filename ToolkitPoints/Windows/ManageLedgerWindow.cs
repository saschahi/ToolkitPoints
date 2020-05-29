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

            if (activeLedger.Points != null && activeLedger.Points.Count > 0)
            {
                selectedViewerName = activeLedger.Points.FirstOrDefault().Key;
                selectedViewerPoints = activeLedger.Points[selectedViewerName];
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
                return null;
            }

            return Viewers.All
                .Where(
                    v => v.Username.Equals(viewerSearch, StringComparison.InvariantCultureIgnoreCase)
                         || v.Username.ToUpper().Contains(viewerSearch.ToUpper())
                )
                // .Take(5)
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

            Rect searchResultButtons = new Rect(204f, searchBox.y + searchBox.height, 200f, 24f);

            foreach (string username in searchResults)
            {
                if (Widgets.ButtonText(searchResultButtons, username))
                {
                    UpdateSelectedViewer(username);
                }

                searchResultButtons.y += searchResultButtons.height;
            }

            bool activeLedgerPointsNull = activeLedger.Points == null;
            int pointKeys = activeLedgerPointsNull ? 0 : activeLedger.Points.Count;

            Rect outRect = new Rect(0, 96, secondColumn.width, secondColumn.height - 150f);
            Rect scrollView = new Rect(0, 0, outRect.width - 20f, (24f * pointKeys) + 16);

            Widgets.DrawMenuSection(outRect);

            Widgets.BeginScrollView(outRect, ref scrollPosition, scrollView);

            Rect row = new Rect(10, 10, scrollView.width - 20f, 24f);

            if (!activeLedgerPointsNull)
            {
                foreach (KeyValuePair<string, int> keyValuePair in activeLedger.Points)
                {
                    DoLedgerRow(row, keyValuePair);
                    row.y += row.height;
                }
            }

            Widgets.EndScrollView();

            GUI.EndGroup();

            Text.Font = original;
        }

        private void DoLedgerRow(Rect row, KeyValuePair<string, int> keyValuePair)
        {
            GUI.BeginGroup(row);

            Rect label = new Rect(0, 0, 300f, 24f);
            Rect pointLabel = new Rect(304f, 0, 200f, 24f);

            Viewer viewer = ViewerController.GetViewer(keyValuePair.Key);

            string viewerLabel = selectedViewerName == keyValuePair.Key
                ? TCText.ColoredText(viewer.DisplayName, ColorLibrary.Gold)
                : viewer.DisplayName;

            if (Widgets.ButtonText(label, viewerLabel, false))
            {
                UpdateSelectedViewer(keyValuePair.Key);
            }

            Widgets.Label(pointLabel, $"{keyValuePair.Value} {ToolkitPointsSettings.pointsBaseName}");

            GUI.EndGroup();
        }


        private void UpdateSelectedViewer(string username)
        {
            if (activeLedger.Points == null)
            {
                activeLedger.Points = new Dictionary<string, int>();
            }

            if (!activeLedger.Points.ContainsKey(username))
            {
                activeLedger.Points.Add(username, 0);
            }

            selectedViewerName = activeLedger.Points.Where((keyPair) => keyPair.Key == username).FirstOrDefault().Key;
            selectedViewerPoints = activeLedger.Points[selectedViewerName];
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
                    activeLedger.Points[selectedViewerName] = selectedViewerPoints;
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
