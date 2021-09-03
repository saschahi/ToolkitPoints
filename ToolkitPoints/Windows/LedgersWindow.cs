using System;
using System.Collections.Generic;
using ToolkitCore.Utilities;
using UnityEngine;
using Verse;

namespace ToolkitPoints.Windows
{
    public class LedgersWindow : Window
    {
        public LedgersWindow()
        {
            this.doCloseButton = true;

            if (Ledgers.Instance.All.Count == 0)
            {
                Log.Message("Detected no ledgers, creating new");
                selectedLedger = Ledger.NewLedger();
            }

            activeLedger = Ledgers.ActiveLedger();

            if (selectedLedger == null)
            {
                selectedLedger = activeLedger;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            GameFont original = Text.Font;
            Text.Font = GameFont.Small;
            float adjustedHeight = inRect.height - 55f - Margin;

            if (inRect.height > adjustedHeight)
            {
                inRect.height = adjustedHeight;
            }

            // Rect regions
            Rect newBtnRegion = new Rect(0f, 0f, inRect.width * 0.4f, Text.LineHeight);
            Rect ledgerMenuRegion = new Rect(
                0f,
                newBtnRegion.height + 5f,
                newBtnRegion.width,
                inRect.height - newBtnRegion.height + 5f
            );
            Rect ledgerSettingsRegion = new Rect(
                ledgerMenuRegion.width + 5f,
                0f,
                inRect.width - ledgerMenuRegion.width - 5f,
                inRect.height
            );
            Rect ledgerMenuCanvas = new Rect(4f, 4f, ledgerMenuRegion.width - 8f, ledgerMenuRegion.height - 8f);
            Rect ledgerMenuViewport = new Rect(
                0f,
                0f,
                ledgerMenuCanvas.width - 16f,
                Text.LineHeight * Ledgers.Instance.All.Count
            );

            
            if (Widgets.ButtonText(newBtnRegion, "New Ledger"))
            {
                Ledger _ = Ledger.NewLedger();
            }

            Rect viewRect = new Rect(0f, 0f, ledgerMenuCanvas.width - 16f, Text.LineHeight * 40f);
            Listing_Standard listing = new Listing_Standard(GameFont.Small);
            Widgets.DrawMenuSection(ledgerMenuRegion);

            GUI.BeginGroup(ledgerMenuRegion);
            //listing.BeginScrollView(ledgerMenuCanvas, ref scrollPosition, ref ledgerMenuViewport);
            Widgets.BeginScrollView(ledgerMenuCanvas, ref scrollPosition, viewRect);
            listing.Begin(viewRect);

            foreach (Ledger ledger in Ledgers.Instance.All)
            {
                Rect lineRect = listing.GetRect(Text.LineHeight);
                Rect iconRect = new Rect(0f, lineRect.y, 24f, lineRect.height);
                Rect ledgerRect = new Rect(26f, lineRect.y, lineRect.width - 26f, lineRect.height);
                bool isActiveLedger = ledger.Id.Equals(ToolkitPointsSettings.currentActiveLedgerId);

                if (!lineRect.IsRegionVisible(ledgerMenuViewport, scrollPosition))
                {
                    continue;
                }

                if (selectedLedger == ledger)
                {
                    Widgets.DrawLightHighlight(ledgerRect);
                }

                Widgets.DrawHighlightIfMouseover(ledgerRect);


                if (isActiveLedger)
                {
                    GUI.DrawTexture(iconRect, Widgets.CheckboxOnTex);
                    TooltipHandler.TipRegion(iconRect, $"{ledger.Name} is currently active");
                }
                else
                {
                    if (Mouse.IsOver(iconRect))
                    {
                        Widgets.CheckboxOnTex.DrawColored(iconRect, new Color(1f, 1f, 1f, 0.38f));
                    }

                    TooltipHandler.TipRegion(iconRect, $@"Set ""{ledger.Name}"" as the current active ledger");

                    if (Widgets.ButtonInvisible(iconRect))
                    {
                        activeLedger = ledger;
                        ToolkitPointsSettings.currentActiveLedgerId = ledger.Id;
                    }
                }

                DoLedgerRow(ledgerRect, ledger);
            }

            GUI.EndGroup();
            //listing.EndScrollView(ref ledgerMenuViewport);
            listing.End();
            Widgets.EndScrollView();


            GUI.BeginGroup(ledgerSettingsRegion);
            DrawLedgerOptions(new Rect(0f, 0f, ledgerSettingsRegion.width, ledgerSettingsRegion.height));
            GUI.EndGroup();
            
            Text.Font = original;
        }

        public override Vector2 InitialSize => new Vector2(500, 500);

        void DoLedgerRow(Rect rect, Ledger ledger)
        {
            GUI.BeginGroup(rect);

            Rect widget = new Rect(0, 0, 200f, 24f);

            string ledgerLabel = ledger == selectedLedger
                ? TCText.ColoredText(ledger.Name, ColorLibrary.Gold)
                : ledger.Name;

            if (Widgets.ButtonText(widget, ledgerLabel, false))
            {
                selectedLedger = ledger;
            }

            GUI.EndGroup();
        }

        private void DrawLedgerOptions(Rect region)
        {
            Listing_Standard listing = new Listing_Standard(GameFont.Small);

            listing.Begin(new Rect(0f, 0f, region.width, Text.LineHeight));
            (Rect nameLabel, Rect nameField) = listing.GetRect(Text.LineHeight).ToForm(0.4f);
            Widgets.Label(nameLabel, "Ledger name");
            selectedLedger.Name = Widgets.TextField(nameField, selectedLedger.Name);

            if (!selectedLedger.Name.NullOrEmpty() && SettingsHelper.DrawClearButton(nameField))
            {
                selectedLedger.Name = selectedLedger.Id.ToString();
            }
            listing.End();

            string manageText = "Manage Ledger";
            string deleteText = "Delete Ledger";
            string resetText = "Reset Ledger";
            float buttonWidth = Mathf.Max(
                Text.CalcSize(manageText).x,
                Text.CalcSize(deleteText).x,
                Text.CalcSize(resetText).x
            ) + 12f;
            
            
            listing.Gap(Text.LineHeight * 2.5f);
            Rect buttonColumnRect = new Rect(
                region.width - buttonWidth,
                listing.CurHeight,
                buttonWidth,
                region.height - listing.CurHeight
            );
            
            
            listing.Begin(buttonColumnRect);

            if (listing.ButtonText(manageText))
            {
                ManageLedgerWindow window = new ManageLedgerWindow(selectedLedger);
                Find.WindowStack.TryRemove(window.GetType());
                Find.WindowStack.Add(window);
            }

            if (listing.ButtonText(deleteText))
            {
                selectedLedger.RemoveLedger();
                activeLedger = Ledgers.ActiveLedger();
                selectedLedger = activeLedger;
            }

            if (listing.ButtonText(resetText))
            {
                selectedLedger.LedgerRecords = new List<LedgerRecord>();
            }
            
            listing.End();
        }

        Vector2 scrollPosition;

        Ledger selectedLedger = null;

        Ledger activeLedger = null;
    }
}
