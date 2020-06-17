using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Verse;

namespace ToolkitPoints
{
    [Serializable]
    public class Ledgers
    {
        public List<Ledger> All { get; set; } = new List<Ledger>();

        public static Ledgers Instance = new Ledgers();

        public static Ledger ActiveLedger()
        {
            Log.Message("Attempting to find an active ledger");

            Ledger current = Instance.All.Find((l) => l.Id == ToolkitPointsSettings.currentActiveLedgerId);

            if (current != null)
            {
                return current;
            }
            else
            {
                Log.Message("Current ledger was null. trying to find default");

                if (Instance.All.Count == 0)
                {
                    Ledger newLedger = Ledger.NewLedger();
                    ToolkitPointsSettings.currentActiveLedgerId = newLedger.Id;

                    return newLedger;
                }
                else
                {
                    return Instance.All.First();
                }
            }
        }
    }
}
