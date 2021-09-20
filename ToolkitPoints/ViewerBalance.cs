using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolkitPoints
{
    [Serializable]
    public class ViewerBalance
    {
        public int Points { get; set; }

        public string Username { get; set; }

        public int Balance()
        {
            return Points;
        }

        public void AddPoints(int points)
        {
            Points += points;
        }

        public void RemovePoints(int points)
        {
            Points -= points;

            if (Points < 0)
            {
                Points = 0;
            }
        }
    }
}
