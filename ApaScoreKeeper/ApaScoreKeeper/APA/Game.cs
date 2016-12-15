using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApaScoreKeeper
{
    public class Game
    {
        Stack<GameEvent> GameEvents { get; set; } = new Stack<GameEvent>();

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public List<Inning> Innings { get; set; } = new List<Inning>();
        public Player ActivePlayer { get; set; }
        public Inning ActiveInning => Innings.Last();
        public List<Player> Players => new List<Player> { Player1, Player2 };
        public bool IsComplete => Points() + DeadBalls() == 10;

        public Game(Player p1, Player p2, Player activePlayer)
        {
            Player1 = p1;
            Player2 = p2;
            ActivePlayer = activePlayer;
            Innings.Add(new Inning(Player1, Player2));
        }

        public void AddPoint()
        {
            ActiveInning.Stats(ActivePlayer).Points++;
            GameEvents.Push(GameEvent.AddPoint);
        }

        public void Safety()
        {
            ActiveInning.Stats(ActivePlayer).Safeties++;
            GameEvents.Push(GameEvent.Safety);
        }

        public void Miss()
        {
            if (ActivePlayer == Player1)
            {
                ActivePlayer = Player2;
            }
            else
            {
                ActivePlayer = Player1;
                Innings.Add(new Inning(Player1, Player2));
            }
            GameEvents.Push(GameEvent.Miss);
        }

        public void DeadBall()
        {
            ActiveInning.Stats(ActivePlayer).DeadBalls++;
            GameEvents.Push(GameEvent.DeadBall);
        }

        public void TimeOut()
        {
            ActiveInning.Stats(ActivePlayer).TimeOuts++;
            GameEvents.Push(GameEvent.TimeOut);
        }

        public void Undo()
        {
            switch(GameEvents.Pop())
            {
                case GameEvent.AddPoint:
                    ActiveInning.Stats(ActivePlayer).Points--;
                    break;
                case GameEvent.Safety:
                    ActiveInning.Stats(ActivePlayer).Safeties--;
                    break;
                case GameEvent.Miss:
                    if (ActivePlayer == Player2)
                    {
                        ActivePlayer = Player1;
                    }
                    else
                    {
                        ActivePlayer = Player2;
                        Innings.RemoveAt(Innings.Count - 1);
                    }
                    break;
                case GameEvent.DeadBall:
                    ActiveInning.Stats(ActivePlayer).DeadBalls--;
                    break;
                case GameEvent.TimeOut:
                    ActiveInning.Stats(ActivePlayer).TimeOuts--;
                    break;
            }
        }

        public int Points(Player player) => Innings.Aggregate(0, (sum, i) => sum + i.Points(player));
        public int Safeties(Player player) => Innings.Aggregate(0, (sum, i) => sum + i.Safeties(player));
        public int DeadBalls(Player player) => Innings.Aggregate(0, (sum, i) => sum + i.DeadBalls(player));
        public int TimeOuts(Player player) => Innings.Aggregate(0, (sum, i) => sum + i.TimeOuts(player));

        public int Points() => Points(Player1) + Points(Player2);
        public int Safeties() => Safeties(Player1) + Safeties(Player2);
        public int DeadBalls() => DeadBalls(Player1) + DeadBalls(Player2);
        public int TimeOuts() => TimeOuts(Player1) + TimeOuts(Player2);
    }

    public enum GameEvent
    {
        AddPoint,
        Safety,
        Miss,
        DeadBall,
        TimeOut,
    }
}
