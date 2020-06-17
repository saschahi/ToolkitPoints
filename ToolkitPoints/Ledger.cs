using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolkitCore.Controllers;
using ToolkitCore.Models;
using Verse;
using static ToolkitCore.Models.Services;

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

        public LedgerRecord GetLedgerRecord(string username, Service service)
        {
            Viewer viewer;

            if (ViewerController.ViewerExists(service, username))
            {
                viewer = ViewerController.GetViewer(service, username);
            }
            else
            {
                viewer = ViewerController.CreateViewer(service, username, 0);
            }

            LedgerRecord ledgerRecord;

            ledgerRecord = LedgerRecords.Find((lr) => lr.Username == username && lr.Service == service);

            if (ledgerRecord == null)
            {
                ledgerRecord = new LedgerRecord()
                {
                    Username = username,
                    Service = service,
                    PointBalance = 0
                };
            }

            return ledgerRecord;
        }

        public int GetViewerBalance(string username, Service service)
        {
            return GetLedgerRecord(username, service).PointBalance;
        }

        public string Name { get; set; }

        public int Id { get; set; }

        public List<LedgerRecord> LedgerRecords { get; set; }
    }
}
