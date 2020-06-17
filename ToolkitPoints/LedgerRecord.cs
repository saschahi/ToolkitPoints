using System;
using static ToolkitCore.Models.Services;

namespace ToolkitPoints
{
    [Serializable]
    public class LedgerRecord
    {
        public string Username { get; set; }

        public Service Service { get; set; }

        public int PointBalance { get; set; }
    }
}
