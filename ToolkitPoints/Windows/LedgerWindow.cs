using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ToolkitPoints.Windows
{
    public class LedgerWindow : Window
    {
        public LedgerWindow()
        {
            Ledger = ToolkitPointsSettings.activeLedger;
        }

        public override void DoWindowContents(Rect inRect)
        {
         
        }

        public override void Close(bool doCloseSound = true)
        {
            base.Close(doCloseSound);

            SaveLoadUtility.SaveAll();
        }

        private Ledger Ledger { get; set; }
    }
}
