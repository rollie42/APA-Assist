using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApaScoreKeeper
{
    public class TeamMatch
    {
        public DateTime StartTime { get; set; }
        public string Location { get; set; }
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<Match> Matches { get; set; } = new List<Match>();
    }
}
