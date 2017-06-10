using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApaScoreKeeper
{
    public class TeamMatch
    {
        public DateTime StartTime => Matches.Min(m => m.StartTime);
        public string Location => HomeTeam.HomeLocation;
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public List<Match> Matches { get; set; } = new List<Match>();
    }
}
