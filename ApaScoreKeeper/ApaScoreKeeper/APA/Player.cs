using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApaScoreKeeper
{
    public class Player
    {
        public string Name { get; set; }
        public int Skill { get; set; }
        public int TimeOutsPerGame => Constants.TimeOutsForSkill[this.Skill];
        public int PointsToWinMatch => Constants.PointsRequiredForSkill[this.Skill];
    }    
}
