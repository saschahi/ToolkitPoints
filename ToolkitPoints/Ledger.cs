using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Verse;

namespace ToolkitPoints
{
    [Serializable]
    public class Ledger
    {
        public static Ledger NewLedger()
        {
            Ledger newLedger = new Ledger();
            newLedger.SetDefaultId();
            newLedger.SetDefaultName();
            Ledgers.Instance.All.Add(newLedger);

            return newLedger;
        }

        public Ledger()
        {
            Name = "NewLedger";
        }

        public void SetDefaultName(int id = 1)
        {
            Ledgers ledgers = Ledgers.Instance;

            if (ledgers.All.Where((l) => l.Name == $"NewLedger {id}").FirstOrDefault() != null)
            {
                SetDefaultName(id + 1);
            }
            else
            {
                Name = $"NewLedger {id}";

                Log.Message($"Created new Ledger with name {Name}");
            }
        }

        public void SetDefaultId(int id = 0)
        {
            Ledgers ledgers = Ledgers.Instance;

            if (ledgers.All.Where((l) => l.Id == id).FirstOrDefault() != null)
            {
                SetDefaultId(id + 1);
            }
            else
            {
                Id = id;

                Log.Message($"Created new ledger with Id {id}");
            }
        }

        public void RemoveLedger()
        {
            if (ToolkitPointsSettings.currentActiveLedgerId == Id)
            {
                if (Ledgers.Instance.All.Count == 1)
                {
                    Ledger newLedger = Ledger.NewLedger();
                    ToolkitPointsSettings.currentActiveLedgerId = newLedger.Id;
                }
                else
                {
                    ToolkitPointsSettings.currentActiveLedgerId = Ledgers.Instance.All.FirstOrDefault().Id;
                }
            }

            Ledgers.Instance.All = Ledgers.Instance.All.Where((x) => x != this).ToList();
        }

        public string Name { get; set; }

        public int Id { get; set; }

        public Dictionary<string, int> Points { get; set; }
    }
}
