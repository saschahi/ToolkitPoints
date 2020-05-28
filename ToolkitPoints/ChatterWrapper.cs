using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolkitPoints
{
    public class ChatterWrapper
    {
#pragma warning disable IDE1006 // Naming Styles
        //public List<string> _links { get; set; }

        public int chatter_count { get; set; }

        public Chatters chatters { get; set; }
    }

    public class Chatters
    {
        public string[] broadcaster { get; set; }

        public string[] vips { get; set; }

        public string[] moderators { get; set; }

        public string[] staff { get; set; }

        public string[] admins { get; set; }

        public string[] global_mods { get; set; }

        public string[] viewers { get; set; }

#pragma warning restore IDE1006 // Naming Styles
    }
}
