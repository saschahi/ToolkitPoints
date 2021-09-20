using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolkitPoints
{
    [Serializable]
    public class Ledger
    {
        public bool Active { get; set; }

        public List<ViewerBalance> Balances { get; set; }

        public string Identifier { get; set; }

        public Ledger()
        {
            Balances = new List<ViewerBalance>();
            Active = true;
            Identifier = "New Ledger";
        }

        public bool IsActive()
        {
            return Active;
        }

        public ViewerBalance GetViewerBalance(string username)
        {
            ViewerBalance viewerBalance = Balances.Find((vwr) => username.Equals(vwr.Username, StringComparison.InvariantCultureIgnoreCase));

            if (viewerBalance != null)
            {
                return viewerBalance;
            }

            viewerBalance = new ViewerBalance()
            {
                Username = username,
                Points = ToolkitPointsSettings.defaultStartingBalance
            };

            Balances.Add(viewerBalance);

            return viewerBalance;
        }
    }
}
