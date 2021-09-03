using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore.Models;

namespace ToolkitPoints
{
    public class ToolkitPointsPermissions : ToolkitCore.Models.PermissionsWrapper
    {
        public override List<Permission> Permissions { get { return permissions; } }

        public override string Namespace { get { return "toolkitpoints"; } }

        List<Permission> permissions = DefaultPermissions();

        static List<Permission> DefaultPermissions()
        {
            return new List<Permission>()
            {
                new Permission("rewardpoints", Role.Mod),
                new Permission("takepoiints", Role.Mod)
            };
        }
    }
}
