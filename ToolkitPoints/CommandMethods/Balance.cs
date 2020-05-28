using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;

namespace ToolkitPoints.CommandMethods
{
    public class Balance : CommandMethod
    {
        public Balance(ToolkitChatCommand command) : base (command)
        {

        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
            TwitchWrapper.SendChatMessage($"@{twitchCommand.Username} you have {Points.Balance(twitchCommand.Username)} {ToolkitPointsSettings.pointsBaseName}");
        }
    }
}
