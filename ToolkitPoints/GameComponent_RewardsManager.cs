using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolkitCore.Models;
using Verse;

namespace ToolkitPoints
{
    class GameComponent_RewardsManager : GameComponent
    {
        public GameComponent_RewardsManager(Game game)
        {

        }

        public override void GameComponentTick()
        {
            if (Current.Game.tickManager.TicksGame % 100 != 0) return;

            TimeSpan timeSinceLastUpdate = DateTime.Now - lastUpdateCycle;

            if (timeSinceLastUpdate.Minutes >= ToolkitPointsSettings.minutesBetweenRewards)
            {
                lastUpdateCycle = DateTime.Now;
                Rewarder.TryRewardingViewers();
            }
        }

        DateTime lastUpdateCycle = DateTime.Now;
    }
}
