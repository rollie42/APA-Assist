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
            Games.Add(new Game(Player1, Player2, Player1));

            // Rebuild match structure (games, innings, etc) if the provided Match already has events
            foreach (var ev in match.GameEvents)
            {
                PerformEvent(ev, false);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Points(Player player) => Games.Aggregate(0, (sum, g) => sum + g.Points(player));
        public int PointsP1 => Points(Player1);
        public int PointsP2 => Points(Player2);

        public int DeadBalls(Player player) => Games.Aggregate(0, (sum, g) => sum + g.DeadBalls(player));
        public int DeadBallsP1 => DeadBalls(Player1);
        public int DeadBallsP2 => DeadBalls(Player2);

        public bool IsComplete { get { return Players.Any(p => Points(p) >= p.PointsToWinMatch); } }

        public int GameNumber => Games.Count();
        public int BallsOnTable => 9 - ActiveGame.Points() - ActiveGame.DeadBalls();

        public Player ActivePlayer => ActiveGame.ActivePlayer;
        public bool ActivePlayerP1 => Player1 == ActivePlayer;
        public bool ActivePlayerP2 => Player2 == ActivePlayer;
        
        public List<Game> Games { get; set; } = new List<Game>();
        public List<Player> Players => new List<Player> { Player1, Player2 };
        public Game ActiveGame => Games.Last();
        
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void ActivePlayerPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName + ActivePlayerSuffix);
        }

        string ActivePlayerSuffix => ActivePlayer == Player1 ? "P1" : "P2";

        #region Operations
        public void AddPoint()
        {
            PerformEvent(GameEvent.AddPoint);
            if (ActiveGame.IsComplete)
            {
                Games.Add(new Game(Player1, Player2, ActivePlayer));
                OnPropertyChanged("GameNumber");
                OnPropertyChanged("InningNumber");
            }
            ActivePlayerPropertyChanged("Points");
            OnPropertyChanged("BallsOnTable");
        }

        public void Safety()
        {
            PerformEvent(GameEvent.Safety);
            ActivePlayerPropertyChanged("Safeties");
        }

        public void Miss()
        {
            PerformEvent(GameEvent.Miss);
            OnPropertyChanged("ActivePlayerP1");
            OnPropertyChanged("ActivePlayerP2");
            OnPropertyChanged("InningNumber");
        }

        public void DeadBall()
        {
            PerformEvent(GameEvent.DeadBall);
            ActivePlayerPropertyChanged("DeadBalls");
            OnPropertyChanged("BallsOnTable");
        }

        public void TimeOut()
        {
            Match.GameEvents.Add(GameEvent.TimeOut);
            ActivePlayerPropertyChanged("T1Visible");
            ActivePlayerPropertyChanged("T2Visible");
        }

        public void Undo()
        {
            if (Match.GameEvents.Count == 0)
            {
                return;
            }

            switch (Match.GameEvents.Last())
            {
                case GameEvent.AddPoint:
                    if (ActiveGame.Points() == 0)
                    {
                        // We're undoing the point that led us to a new game
                        Games.RemoveAt(Games.Count - 1);
                        ActiveGame.Undo();
                        OnPropertyChanged("GameNumber");
                        OnPropertyChanged("InningNumber");
                    }
                    else
                    {
                        ActiveGame.Undo();
                    }

                    ActivePlayerPropertyChanged("Points");
                    OnPropertyChanged("BallsOnTable");
                    break;
                case GameEvent.Safety:
                    ActiveGame.Undo();
                    ActivePlayerPropertyChanged("Safeties");
                    break;
                case GameEvent.Miss:
                    ActiveGame.Undo();
                    OnPropertyChanged("ActivePlayerP1");
                    OnPropertyChanged("ActivePlayerP2");
                    OnPropertyChanged("InningNumber");
                    break;
                case GameEvent.DeadBall:
                    ActiveGame.Undo();
                    ActivePlayerPropertyChanged("DeadBalls");
                    OnPropertyChanged("BallsOnTable");
                    break;
                case GameEvent.TimeOut:
                    ActiveGame.Undo();
                    break;
            }

            Match.GameEvents.RemoveAt(Match.GameEvents.Count - 1);
        }

        void PerformEvent(GameEvent ev, bool updateHistory = true)
        {
            switch (ev)
            {
                case GameEvent.AddPoint:
                    ActiveGame.AddPoint();
                    break;
                case GameEvent.Safety:
                    ActiveGame.Safety();
                    break;
                case GameEvent.Miss:
                    ActiveGame.Miss();
                    break;
                case GameEvent.DeadBall:
                    ActiveGame.DeadBall();
                    break;
                case GameEvent.TimeOut:
                    ActiveGame.TimeOut();
                    break;
            }

            if (updateHistory)
            {
                Match.GameEvents.Add(ev);
            }
        }
        #endregion
    }
}
