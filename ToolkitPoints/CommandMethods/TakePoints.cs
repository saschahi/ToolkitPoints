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
    public class TakePoints : CommandMethod
    {
        public TakePoints(ToolkitChatCommand command) : base(command)
        {

        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
            List<string> args = twitchCommand.ArgumentsAsList;

            if (args.Count < 2)
            {
                TwitchWrapper.SendChatMessage($"@{twitchCommand.Username} to take {ToolkitPointsSettings.pointsBaseName} make sure to include a username then an amount.");
                return;
            }

            string username = args[0].Replace("@", "");

            if (!int.TryParse(args[1], out int points))
            {
                TwitchWrapper.SendChatMessage($"@{twitchCommand.Username} to take {ToolkitPointsSettings.pointsBaseName} make sure to include the amount after the username.");
                return;
            }

            Points.RemovePoints(username, points);

            TwitchWrapper.SendChatMessage($"@{username} you have had {points} {ToolkitPointsSettings.pointsBaseName} taken from you by @{twitchCommand.Username}");
        }
    }
}
