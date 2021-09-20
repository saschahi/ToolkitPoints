using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore.Database;
using ToolkitCore.Utilities;
using Verse;

namespace ToolkitPoints
{
    [StaticConstructorOnStartup]
    static class SaveLoadUtility
    {
        static readonly string ledgerFileName = "Ledger";

        static readonly Mod mod = LoadedModManager.GetMod<ToolkitPoints>();

        static SaveLoadUtility()
        {
            LoadAll();
        }

        internal static void SaveAll()
        {
            DatabaseController.SaveObject(ToolkitPointsSettings.allLedgers, ledgerFileName, mod);
            mod.WriteSettings();
        }

        internal static void LoadAll()
        {
            Log.Message(TCText.ColoredText("Trying to load previous ledger file", ColorLibrary.BrightBlue));

            if (DatabaseController.LoadObject(ledgerFileName, mod, out List<Ledger> ledgers))
            {
                Log.Message("Ledger file found, loading ledgers from file");

                ToolkitPointsSettings.allLedgers = ledgers;

                // If no ledgers exist, create a new one
                if (ToolkitPointsSettings.allLedgers.Count < 1)
                {
                    ToolkitPointsSettings.allLedgers.Add(new Ledger());
                }

                Ledger activeLedger = ToolkitPointsSettings.allLedgers.Find((ledger) => ledger.Active);

                // If no ledgers are active, make the first one active
                if (activeLedger == null)
                {
                    activeLedger = ToolkitPointsSettings.allLedgers.First();
                    activeLedger.Active = true;
                }

                ToolkitPointsSettings.activeLedger = activeLedger;
            } else
            {
                ToolkitPointsSettings.allLedgers = new List<Ledger>();
                ToolkitPointsSettings.allLedgers.Add(new Ledger());
                ToolkitPointsSettings.activeLedger = ToolkitPointsSettings.allLedgers.First();
            }
        }
    }
}
