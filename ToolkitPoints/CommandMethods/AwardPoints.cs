using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore;
using ToolkitCore.Interfaces;
using ToolkitCore.Models;
using ToolkitCore.Utilities;
using TwitchLib.Client.Interfaces;
using Verse;

namespace ToolkitPoints.CommandMethods
{
    public class AwardPoints : CommandMethod
    {
        public AwardPoints(ToolkitChatCommand command) : base(command)
        {

        }

        public override void Execute(ICommand command)
        {
            List<string> args = command.Parameters(); 

            if (args.Count < 2)
            {
                TwitchWrapper.SendChatMessage($"@{command.Username()} to award {ToolkitPointsSettings.pointsBaseName} make sure to include a username then an amount.");
                return;
            }

            string username = args[1].Replace("@", "");
            //Remove
            Log.Message("points to reward: " + args[2]);
            if (!int.TryParse(args[2], out int points))
            {
                TwitchWrapper.SendChatMessage($"@{command.Username()} to award {ToolkitPointsSettings.pointsBaseName} make sure to include the amount after the username.");
                return;
            }

            ViewerBalance viewerBalance = ToolkitPointsSettings.activeLedger.GetViewerBalance(username);
            viewerBalance.AddPoints(points);

            MessageSender.SendMessage($"@{username} you have been awarded {points} {ToolkitPointsSettings.pointsBaseName} from @{command.Username()}," +
                $" giving you a new balance of {viewerBalance.Points} {ToolkitPointsSettings.pointsBaseName}"
                , command.Service());
        }
    }
}
