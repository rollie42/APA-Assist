using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApaScoreKeeper.ViewModels
{
    public class MatchVM : INotifyPropertyChanged
    {
        public Match Match { get; set; }
        public Player Player1 => Match.Player1;
        public Player Player2 => Match.Player2;
        public MatchVM(Match match)
        {
            Match = match;
            Match.PropertyChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, e);
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Points(Player player) => Match.Games.Aggregate(0, (sum, g) => sum + g.Points(player));
        public int PointsP1 => Points(Player1);
        public int PointsP2 => Points(Player2);

        public int DeadBalls(Player player) => Match.Games.Aggregate(0, (sum, g) => sum + g.DeadBalls(player));
        public int DeadBallsP1 => DeadBalls(Player1);
        public int DeadBallsP2 => DeadBalls(Player2);

        public bool IsComplete { get { return Match.Players.Any(p => Points(p) >= p.PointsToWinMatch); } }

        public int GameNumber => Match.Games.Count();
        public int BallsOnTable => 9 - ActiveGame.Points() - ActiveGame.DeadBalls();

        public Player ActivePlayer => ActiveGame.ActivePlayer;
        public bool ActivePlayerP1 => Player1 == ActivePlayer;
        public bool ActivePlayerP2 => Player2 == ActivePlayer;
        public Game ActiveGame => Match.ActiveGame;

    }
}
