using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ToolkitPoints
{
    public static class Points
    {
        public static void RemovePoints(string username, int points)
        {
            if (points < 1)
            {
                Log.Error("Cannot remove negative or zero points");
                return;
            }

            Ledger ledger = Ledgers.ActiveLedger();

            if (!ledger.Points.ContainsKey(username))
            {
                ledger.Points.Add(username, 0);
            }

            ledger.Points[username] -= points;

            if (ledger.Points[username] < 0)
            {
                ledger.Points[username] = 0;
            }
        }

        public static void AddPoints(string username, int points)
        {
            if (points < 1)
            {
                Log.Error("Cannot add negative or zero points");
                return;
            }

            Ledger ledger = Ledgers.ActiveLedger();

            if (!ledger.Points.ContainsKey(username))
            {
                ledger.Points.Add(username, 0);
            }

            ledger.Points[username] += points;
        }

        public static int Balance(string username)
        {
            Ledger ledger = Ledgers.ActiveLedger();

            if (!ledger.Points.ContainsKey(username))
            {
                ledger.Points.Add(username, 0);
            }

            return ledger.Points[username];
        }
    }
}
