using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApaScoreKeeper
{
    public class Inning
    {
        //public Dictionary<Player, PlayerInningStats> Players { get; set; } = new Dictionary<Player, PlayerInningStats>();
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public PlayerInningStats Player1Stats { get; set; } = new PlayerInningStats();
        public PlayerInningStats Player2Stats { get; set; } = new PlayerInningStats();

        public int Points(Player player) => Stats(player).Points;
        public int Safeties(Player player) => Stats(player).Safeties;
        public int DeadBalls(Player player) => Stats(player).DeadBalls;
        public int TimeOuts(Player player) => Stats(player).TimeOuts;

        public Inning(Player p1, Player p2)
        {
            Player1 = p1;
            Player2 = p2;
        }

        public PlayerInningStats Stats(Player player)
        {
            return player == Player1 ? Player1Stats : Player2Stats;
        }
    }

    public class PlayerInningStats
    {
        public int Points { get; set; }
        public int Safeties { get; set; }
        public int DeadBalls { get; set; }
        public int TimeOuts { get; set; }
    }
}
