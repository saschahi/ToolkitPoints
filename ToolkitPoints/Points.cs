using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static ToolkitCore.Models.Services;

namespace ToolkitPoints
{
    public static class Points
    {
        public static void RemovePoints(string username, Service service, int points)
        {
            if (points < 1)
            {
                Log.Error("Cannot remove negative or zero points");
                return;
            }

            Ledger ledger = Ledgers.ActiveLedger();

            LedgerRecord ledgerRecord = ledger.GetLedgerRecord(username, service);

            ledgerRecord.PointBalance -= points;
        }

        public static void AddPoints(string username, Service service, int points)
        {
            if (points < 1)
            {
                Log.Error("Cannot add negative or zero points");
                return;
            }

            Ledger ledger = Ledgers.ActiveLedger();

            LedgerRecord ledgerRecord = ledger.GetLedgerRecord(username, service);

            ledgerRecord.PointBalance += points;
        }

        public static int Balance(string username, Service service)
        {
            return Ledgers.ActiveLedger().GetViewerBalance(username, service);
        }
    }
}
