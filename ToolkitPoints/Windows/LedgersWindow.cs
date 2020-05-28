using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            GameFont original = Text.Font;
            Text.Font = GameFont.Small;

            Rect firstColumn = new Rect(0, 0, inRect.width / 2f, inRect.height);
            Rect secondColumn = new Rect(firstColumn.width, 0, firstColumn.width, inRect.height);

            // First Column

            GUI.BeginGroup(firstColumn);

            Rect createNew = new Rect(0, 0, 200f, 24f);

            if (Widgets.ButtonText(createNew, "New Ledger"))
            {
                Ledger newLedger = Ledger.NewLedger();
            }

            Rect makeLedgerActive = new Rect(0, 40f, 200f, 24f);

            if (selectedLedger != activeLedger)
            {
                if (Widgets.ButtonText(makeLedgerActive, "Make Active Ledger"))
                {
                    activeLedger = selectedLedger;
                    ToolkitPointsSettings.currentActiveLedgerId = activeLedger.Id;
                }
            }
            else
            {
                Widgets.Label(makeLedgerActive, TCText.ColoredText($"{activeLedger.Name} is active", ColorLibrary.BrightGreen));
            }

            Rect ledgerName = new Rect(0, makeLedgerActive.y + 28f, 200f, 24f);

            selectedLedger.Name = Widgets.TextField(ledgerName, selectedLedger.Name);

            Rect manageLedger = new Rect(0, ledgerName.y + 28f, 200f, 24f);

            if (Widgets.ButtonText(manageLedger, "Manage Ledger"))
            {
                ManageLedgerWindow window = new ManageLedgerWindow(selectedLedger);
                Find.WindowStack.TryRemove(window.GetType());
                Find.WindowStack.Add(window);
            }

            Rect deleteLedger = new Rect(0, manageLedger.y + 40f, 200f, 24f);

            if (Widgets.ButtonText(deleteLedger, "Delete Ledger"))
            {
                selectedLedger.RemoveLedger();
                activeLedger = Ledgers.ActiveLedger();
                selectedLedger = activeLedger;
            }

            Rect resetLedger = new Rect(0, deleteLedger.y + 28f, 200f, 24f);

            if (Widgets.ButtonText(resetLedger, "Reset Ledger"))
            {
                selectedLedger.Points = new Dictionary<string, int>();
            }

            GUI.EndGroup();

            // Second Column

            GUI.BeginGroup(secondColumn);

            Rect outRect = new Rect(0, 0, secondColumn.width, secondColumn.height - 60f);
            Rect scrollView = new Rect(0, 0, outRect.width - 16f, 24f * Ledgers.Instance.All.Count);

            Widgets.BeginScrollView(outRect, ref scrollPosition, scrollView);

            Rect row = new Rect(0, 0, scrollView.width - 20f, 24f);

            foreach (Ledger ledger in Ledgers.Instance.All)
            {
                DoLedgerRow(row, ledger);
                row.y += row.height;
            }

            Widgets.EndScrollView();


            GUI.EndGroup();

            Text.Font = original;
        }

        public override Vector2 InitialSize => new Vector2(500, 500);

        void DoLedgerRow(Rect rect, Ledger ledger)
        {
            GUI.BeginGroup(rect);

            Rect widget = new Rect(0, 0, 200f, 24f);

            string ledgerLabel = ledger == selectedLedger ? TCText.ColoredText(ledger.Name, ColorLibrary.Gold) : ledger.Name;

            if (Widgets.ButtonText(widget, ledgerLabel, false))
            {
                selectedLedger = ledger;
            }

            GUI.EndGroup();
        }

        Vector2 scrollPosition;

        Ledger selectedLedger = null;

        Ledger activeLedger = null;
    }
}
