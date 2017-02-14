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
        public List<Player> Players => new List<Player> { Player1, Player2 };
        public Game ActiveGame => Games.Last();
        public Player ActivePlayer => ActiveGame.ActivePlayer;

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
        
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void ActivePlayerPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName + ActivePlayerSuffix);
        }

        string ActivePlayerSuffix => ActivePlayer == Player1 ? "P1" : "P2";
        
    }
}
