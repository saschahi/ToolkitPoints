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
    static class SaveConfig
    {
        static readonly string ledgerFileName = "Ledger";

        static readonly Mod mod = LoadedModManager.GetMod<ToolkitPoints>();

        static SaveConfig()
        {
            LoadAll();
        }

        internal static void SaveAll()
        {
            DatabaseController.SaveObject(Ledgers.Instance, ledgerFileName, mod);
            mod.WriteSettings();
        }

        internal static void LoadAll()
        {
            Log.Message(TCText.ColoredText("Trying to load previous ledger file", ColorLibrary.BrightBlue));

            if (DatabaseController.LoadObject(ledgerFileName, mod, out Ledgers ledgers))
            {
                Log.Message("Ledger file found, loading ledgers from file");

                Ledgers.Instance = ledgers;
            }
        }
    }
}
