using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolkitPoints
{
    public class ChattersObject
    {
        public int chatters_count;

        public Chatters chatters;
    }

    public class Chatters
    {
        public string[] broadcaster;

        public string[] vips;

        public string[] moderators;

        public string[] staff;

        public string[] admins;

        public string[] global_mods;

        public string[] viewers;
    }
}
