using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApaScoreKeeper.ViewModels
{
    public class LeagueMatchVM : MatchVM
    {
        public LeagueMatchVM(Match match) : base(match)
        {

        }
        
        public int TimeOuts(Player player) => Games.Aggregate(0, (sum, g) => sum + g.TimeOuts(player));
        public int TimeOutsP1 => TimeOuts(Player1);
        public int TimeOutsP2 => TimeOuts(Player2);
        
        public int Safeties(Player player) => Games.Aggregate(0, (sum, g) => sum + g.Safeties(player));
        public int SafetiesP1 => Safeties(Player1);
        public int SafetiesP2 => Safeties(Player2);

        public bool T1VisibleP1 => Player1.TimeOutsPerGame - ActiveGame.TimeOuts(Player1) > 0;
        public bool T2VisibleP1 => Player1.TimeOutsPerGame - ActiveGame.TimeOuts(Player1) > 1;
        public bool T1VisibleP2 => Player2.TimeOutsPerGame - ActiveGame.TimeOuts(Player2) > 0;
        public bool T2VisibleP2 => Player2.TimeOutsPerGame - ActiveGame.TimeOuts(Player2) > 1;

        public int InningNumber => ActiveGame.Innings.Count();


    }
}
