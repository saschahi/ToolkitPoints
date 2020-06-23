using System;
using System.Collections.Generic;
using System.Linq;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using UnityEngine;
using Verse;

namespace ToolkitPoints.Windows
{
    [StaticConstructorOnStartup]
    public class ManageLedgerWindow : Window
    {
        private readonly Ledger activeLedger;
        private RecordKeeper currentRecord;
        private bool hasMultipleServices;
        private bool insensitiveMatchFound;
        private Services.Service ledgerService;
        private string lastCachedSearch = "";
        private static Dictionary<Services.Service, Texture2D> serviceIcons = new Dictionary<Services.Service, Texture2D>();

        private Vector2 scrollPosition = Vector2.zero;

        private List<Tuple<string, Services.Service>> searchResults = new List<Tuple<string, Services.Service>>();

        private string viewerSearch = "";

        static ManageLedgerWindow()
        {
            foreach (string name in Enum.GetNames(typeof(Services.Service)))
            {
                Services.Service service = (Services.Service) Enum.Parse(typeof(Services.Service), name);

                serviceIcons[service] = ContentFinder<Texture2D>.Get($"UI/Icons/{name.ToLower()}Icon");
            }
        }


        public ManageLedgerWindow(Ledger ledger = null)
        {
            forcePause = true;
            doCloseButton = true;
            onlyOneOfTypeAllowed = true;
            activeLedger = ledger ?? Ledgers.ActiveLedger();

            if (activeLedger.LedgerRecords == null)
            {
                activeLedger.LedgerRecords = new List<LedgerRecord>();
            }

            if (activeLedger.LedgerRecords.Count > 0)
            {
                currentRecord = new RecordKeeper(activeLedger.LedgerRecords.First());
            }
        }

        public override Vector2 InitialSize => new Vector2(1000, 600);

        public override void PreOpen()
        {
            base.PreOpen();
            
            Notify_RecacheRequested();
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();
            currentRecord?.CommitChangesIfAny();

            if (!(Time.time % 2 < 1))
            {
                return;
            }

            Notify_RecacheRequested();

            if (!lastCachedSearch.Equals(viewerSearch))
            {
                Notify_SearchRequested();
            }
        }

        private void Notify_SearchRequested()
        {
            searchResults = GetSearchResults();
            lastCachedSearch = viewerSearch;
            insensitiveMatchFound = searchResults
                .Any(pair => pair.Item1.EqualsIgnoreCase(viewerSearch));
        }

        private void Notify_RecacheRequested()
        {
            HashSet<Services.Service> services = activeLedger.LedgerRecords
                .Select(l => l.Service)
                .Distinct()
                .ToHashSet();

            hasMultipleServices = services.Count > 1;
            ledgerService = services.FirstOrFallback();
        }

        private List<Tuple<string, Services.Service>> GetSearchResults()
        {
            if (viewerSearch.NullOrEmpty())
            {
                return new List<Tuple<string, Services.Service>>();
            }

            return activeLedger.LedgerRecords
                .Where(
                    v => v.Username.Equals(viewerSearch, StringComparison.InvariantCultureIgnoreCase)
                         || v.Username.ToUpper().Contains(viewerSearch.ToUpper())
                )
                // .Take(3)
                .Select(v => new Tuple<string, Services.Service>(v.Username, v.Service))
                .ToList();
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }
            
            float adjustedHeight = inRect.height - 55f;

            if (inRect.height > adjustedHeight)
            {
                inRect.height = adjustedHeight;
            }
            
            Listing_Standard listing = new Listing_Standard(GameFont.Small);
            Rect leftColumnRect = new Rect(0f, 0f, (inRect.width / 2f) - 5f, inRect.height);
            Rect rightColumnRect = new Rect(leftColumnRect.width + 10f, 0f, leftColumnRect.width, inRect.height);
            Rect columnInnerRect = new Rect(0f, 0f, leftColumnRect.width, inRect.height - Text.LineHeight - 6f);
            Rect listRect = new Rect(
                rightColumnRect.x,
                Text.LineHeight + 6f,
                rightColumnRect.width,
                rightColumnRect.height - Text.LineHeight - 6f
            );
            Rect listView = new Rect(0f, 0f, listRect.width - 16f - 8f, listRect.height);

            listing.Begin(leftColumnRect);
            
            if(currentRecord != null)
            {
                DrawViewerMenu(listing);
            }

            listing.End();
            
            listing.Begin(new Rect(rightColumnRect.x, rightColumnRect.y, rightColumnRect.width, Text.LineHeight));
            DrawSearchBox(listing);
            listing.End();
            
            Widgets.DrawMenuSection(listRect);
            listing.Begin(listRect);
            Rect listInnerRect = new Rect(0f, 0f, listRect.width, listRect.height);

            if (searchResults.Count > 0)
            {
                listView.height = searchResults.Count * Text.LineHeight;
                DrawSearchResults(listing, listInnerRect.ContractedBy(4f), ref listView);
            }
            else
            {
                listView.height = activeLedger.LedgerRecords.Count * Text.LineHeight + Text.LineHeight;
                DrawLedgerRecords(listing, listInnerRect.ContractedBy(4f), ref listView);
            }
            
            listing.End();
        }

        private void DrawServiceIcon(Rect canvas, Services.Service service)
        {
            if (!hasMultipleServices)
            {
                return;
            }

            if (!serviceIcons.TryGetValue(service, out Texture2D icon))
            {
                icon = Texture2D.whiteTexture;
            }
            
            GUI.DrawTexture(new Rect(canvas.x + 4f, canvas.y + 4f, 14f, 16f), icon);
        }
        
        private void DrawSearchBox(Listing listing)
        {
            (Rect searchLabel, Rect searchField) = listing.GetRect(Text.LineHeight).RightHalf().ToForm(0.3f);
            Widgets.Label(searchLabel, "Search:");
            viewerSearch = Widgets.TextField(searchField, viewerSearch);

            if (!viewerSearch.NullOrEmpty())
            {
                if (SettingsHelper.DrawClearButton(searchField))
                {
                    viewerSearch = "";
                    lastCachedSearch = "";
                }
            }
            else
            {
                if(searchResults.Count > 0)
                {
                    searchResults = new List<Tuple<string, Services.Service>>();
                }
            }
        }

        private void DrawSearchResults(Listing_Standard listing, Rect area, ref Rect view)
        {
            if (!insensitiveMatchFound)
            {
                Rect addLine = listing.GetRect(Text.LineHeight);

                if (addLine.IsRegionVisible(view, scrollPosition))
                {
                    Widgets.Label(addLine, $@"Create ""{viewerSearch}""");

                    if (Widgets.ButtonInvisible(addLine))
                    {
                        currentRecord = RequestAccount(viewerSearch);
                    }
                }
            }

            listing.BeginScrollView(new Rect(area.x, area.y + Text.LineHeight, area.width, area.height - Text.LineHeight), ref scrollPosition, ref view);
            for (int index = 0; index < searchResults.Count; index++)
            {
                (string name, Services.Service service) = searchResults[index];
                
                Rect lineRect = listing.GetRect(Text.LineHeight);
                Rect iconRect = new Rect(0f, lineRect.y, 24f, lineRect.height);
                Rect nameRect = new Rect(
                    hasMultipleServices ? iconRect.width + 5f : 0f,
                    lineRect.y,
                    lineRect.width - iconRect.width - 5f,
                    lineRect.height
                );

                if (!lineRect.IsRegionVisible(view, scrollPosition))
                {
                    continue;
                }

                if (index % 2 == 1)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                if (currentRecord?.IsUser(service, name) ?? false)
                {
                    Widgets.DrawHighlightSelected(lineRect);
                }

                if(hasMultipleServices)
                {
                    DrawServiceIcon(iconRect, service);
                }

                Widgets.Label(nameRect, name);

                if (Widgets.ButtonInvisible(lineRect))
                {
                    currentRecord = RequestAccount(name, service);
                }
            }
            listing.EndScrollView(ref view);
        }

        private void DrawLedgerRecords(Listing_Standard listing, Rect area, ref Rect view)
        {
            Rect headerRect = listing.GetRect(Text.LineHeight);
            Rect iconHeaderRect = new Rect(0f, headerRect.y, 24f, Text.LineHeight);
            Rect ledgerHeaderRect = new Rect(
                hasMultipleServices ? iconHeaderRect.width + 1f : 0f,
                headerRect.y,
                headerRect.width - (hasMultipleServices ? iconHeaderRect.width : 0f),
                Text.LineHeight
            );
            (Rect nameHeaderRect, Rect pointsHeaderRect) = ledgerHeaderRect.ToForm(0.55f);

            Widgets.Label(new Rect(nameHeaderRect.x + 4f, nameHeaderRect.y, nameHeaderRect.width - 4f, nameHeaderRect.height), "Username");
            Widgets.Label(new Rect(pointsHeaderRect.x + 4f, pointsHeaderRect.y, pointsHeaderRect.width - 4f, pointsHeaderRect.height), ToolkitPointsSettings.pointsBaseName);

            listing.BeginScrollView(new Rect(area.x, area.y + Text.LineHeight, area.width, area.height - Text.LineHeight), ref scrollPosition, ref view);
            for (int index = 0; index < activeLedger.LedgerRecords.Count; index++)
            {
                LedgerRecord record = activeLedger.LedgerRecords[index];
                Rect lineRect = listing.GetRect(Text.LineHeight);
                Rect iconRect = new Rect(0f, lineRect.y, iconHeaderRect.width, lineRect.height);
                Rect nameRect = new Rect(nameHeaderRect.x, lineRect.y, nameHeaderRect.width, lineRect.height);
                Rect pointsRect = new Rect(pointsHeaderRect.x, lineRect.y, pointsHeaderRect.width, lineRect.height);

                if (!lineRect.IsRegionVisible(view, scrollPosition))
                {
                    continue;
                }

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                if (currentRecord?.IsUser(record) ?? false)
                {
                    Widgets.DrawHighlightSelected(lineRect);
                }

                if (hasMultipleServices)
                {
                    DrawServiceIcon(iconRect, record.Service);
                }
                
                if (Text.CalcSize(record.Username).x > nameRect.width)
                {
                    Widgets.Label(nameRect, $"{record.Username.Substring(0, 7)}...");
                    TooltipHandler.TipRegion(nameRect, record.Username);
                }
                else
                {
                    Widgets.Label(nameRect, record.Username);
                }
                
                Widgets.Label(pointsRect, record.PointBalance.ToString("N0"));

                if (Widgets.ButtonInvisible(lineRect))
                {
                    currentRecord = new RecordKeeper(record);
                }
            }
            listing.EndScrollView(ref view);
        }

        private void DrawViewerMenu(Listing listing)
        {
            (Rect nameLabel, Rect nameField) = listing.GetRect(Text.LineHeight).ToForm(0.7f);
            Rect iconRect = new Rect(nameField.x, nameField.y, 24f, nameField.height);

            if (hasMultipleServices)
            {
                nameField.x += 28f;
                nameField.width -= 28f;
                DrawServiceIcon(iconRect, currentRecord.record.Service);
            }

            Widgets.Label(nameLabel, "Username");
            Widgets.Label(nameField, currentRecord.record.Username);

            (Rect pointsLabel, Rect pointsField) = listing.GetRect(Text.LineHeight).ToForm(0.7f);
            Widgets.Label(pointsLabel, ToolkitPointsSettings.pointsBaseName);
            Widgets.TextFieldNumeric(pointsField, ref currentRecord.pointsIntermediate, ref currentRecord.pointsCached);

            (Rect addLabel, Rect addField) = listing.GetRect(Text.LineHeight).ToForm(0.7f);
            Widgets.Label(addLabel, $"Give {ToolkitPointsSettings.pointsBaseName.ToLowerInvariant()}");
            Widgets.TextFieldNumeric(addField, ref currentRecord.pointsToAdd, ref currentRecord.pointsToAddCached);

            if (currentRecord.pointsToAdd > 0 && SettingsHelper.DrawDoneButton(addField))
            {
                currentRecord.pointsIntermediate += currentRecord.pointsToAdd;
            }

            (Rect removeLabel, Rect removeField) = listing.GetRect(Text.LineHeight).ToForm(0.7f);
            Widgets.Label(removeLabel, $"Take {ToolkitPointsSettings.pointsBaseName.ToLowerInvariant()}");
            Widgets.TextFieldNumeric(removeField, ref currentRecord.pointsToRemove, ref currentRecord.pointsToRemoveCached);

            if (currentRecord.pointsToRemove > 0 && SettingsHelper.DrawDoneButton(removeField))
            {
                currentRecord.pointsIntermediate -= currentRecord.pointsToRemove;
            }
        }

        private RecordKeeper RequestAccount(string username)
        {
            return RequestAccount(username, ledgerService);
        }

        private RecordKeeper RequestAccount(string username, Services.Service service)
        {
            RecordKeeper keeper = new RecordKeeper(activeLedger.GetLedgerRecord(username, service));

            if (activeLedger.LedgerRecords.Find(
                r => r.Service == keeper.record.Service && r.Username.EqualsIgnoreCase(keeper.record.Username)
            ) == null)
            {
                activeLedger.LedgerRecords.Add(keeper.record);
            }

            return keeper;
        }


        private class RecordKeeper
        {
            public string pointsCached;
            public string pointsToAddCached;
            public string pointsToRemoveCached;
            public int pointsIntermediate;
            public int pointsToAdd;
            public int pointsToRemove;
            public readonly LedgerRecord record;

            public RecordKeeper(LedgerRecord record)
            {
                this.record = record;
                pointsIntermediate = record.PointBalance;

                pointsToAdd = 0;
                pointsToRemove = 0;
                pointsToAddCached = "0";
                pointsToRemoveCached = "0";
            }

            public string Points => pointsCached;

            public bool IsUser(LedgerRecord other)
            {
                return IsUser(other.Service, other.Username);
            }

            public bool IsUser(Services.Service service, string username)
            {
                return record.Service == service && record.Username.EqualsIgnoreCase(username);
            }

            public void CommitChangesIfAny()
            {
                if (pointsIntermediate == record.PointBalance)
                {
                    return;
                }

                record.PointBalance = pointsIntermediate;
                pointsCached = record.PointBalance.ToString();
            }
        }
    }
}
