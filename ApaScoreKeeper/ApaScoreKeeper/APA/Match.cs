using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ApaScoreKeeper
{
    public class Match : INotifyPropertyChanged
    {
        Stack<GameEvent> GameEvents { get; set; } = new Stack<GameEvent>();

        public event PropertyChangedEventHandler PropertyChanged;
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public List<Game> Games { get; set; } = new List<Game>();
        public Player ActivePlayer => ActiveGame.ActivePlayer;
        public bool ActivePlayerP1 => Player1 == ActivePlayer;
        public bool ActivePlayerP2 => Player2 == ActivePlayer;
        public Game ActiveGame => Games.Last();
        public List<Player> Players => new List<Player> { Player1, Player2 };

        public Match(Player p1, Player p2)
        {
            Player1 = p1;
            Player2 = p2;
            Games.Add(new Game(Player1, Player2, Player1));
        }

        #region Operations
        public void AddPoint()
        {
            ActiveGame.AddPoint();
            if (ActiveGame.IsComplete)
            {
                Games.Add(new Game(Player1, Player2, ActivePlayer));
                OnPropertyChanged("GameNumber");
                OnPropertyChanged("InningNumber");
            }
            ActivePlayerPropertyChanged("Points");
            OnPropertyChanged("BallsOnTable");
            GameEvents.Push(GameEvent.AddPoint);
        }

        public void Safety()
        {
            ActiveGame.Safety();
            ActivePlayerPropertyChanged("Safeties");
            GameEvents.Push(GameEvent.Safety);
        }

        public void Miss()
        {
            ActiveGame.Miss();
            OnPropertyChanged("ActivePlayerP1");
            OnPropertyChanged("ActivePlayerP2");
            OnPropertyChanged("InningNumber");
            GameEvents.Push(GameEvent.Miss);
        }

        public void DeadBall()
        {
            ActiveGame.DeadBall();
            ActivePlayerPropertyChanged("DeadBalls");
            OnPropertyChanged("BallsOnTable");
            GameEvents.Push(GameEvent.DeadBall);
        }

        public void TimeOut()
        {
            ActiveGame.TimeOut();
            ActivePlayerPropertyChanged("T1Visible");
            ActivePlayerPropertyChanged("T2Visible");
            GameEvents.Push(GameEvent.TimeOut);
        }

        public void Undo()
        {
            if (GameEvents.Count == 0)
            {
                return;
            }

            switch (GameEvents.Pop())
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
        }
        #endregion

        public int Points(Player player) => Games.Aggregate(0, (sum, g) => sum + g.Points(player));
        public int PointsP1 => Points(Player1);
        public int PointsP2 => Points(Player2);

        public int Safeties(Player player) => Games.Aggregate(0, (sum, g) => sum + g.Safeties(player));
        public int SafetiesP1 => Safeties(Player1);
        public int SafetiesP2 => Safeties(Player2);

        public int DeadBalls(Player player) => Games.Aggregate(0, (sum, g) => sum + g.DeadBalls(player));
        public int DeadBallsP1 => DeadBalls(Player1);
        public int DeadBallsP2 => DeadBalls(Player2);

        public int TimeOuts(Player player) => Games.Aggregate(0, (sum, g) => sum + g.TimeOuts(player));
        public int TimeOutsP1 => TimeOuts(Player1);
        public int TimeOutsP2 => TimeOuts(Player2);
        public bool T1VisibleP1 => Player1.TimeOutsPerGame - ActiveGame.TimeOuts(Player1) > 0;
        public bool T2VisibleP1 => Player1.TimeOutsPerGame - ActiveGame.TimeOuts(Player1) > 1;
        public bool T1VisibleP2 => Player2.TimeOutsPerGame - ActiveGame.TimeOuts(Player2) > 0;
        public bool T2VisibleP2 => Player2.TimeOutsPerGame - ActiveGame.TimeOuts(Player2) > 1;

        public bool IsComplete { get { return Players.Any(p => Points(p) == p.PointsToWinMatch); } }
        
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void ActivePlayerPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName + ActivePlayerSuffix);
        }

        string ActivePlayerSuffix => ActivePlayer == Player1 ? "P1" : "P2";
        public int GameNumber => Games.Count();
        public int InningNumber => ActiveGame.Innings.Count();
        public int BallsOnTable => 9 - ActiveGame.Points() - ActiveGame.DeadBalls();
    }
}
