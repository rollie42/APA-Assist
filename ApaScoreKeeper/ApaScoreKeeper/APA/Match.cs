using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ApaScoreKeeper
{
    public class Match : IUniqueIdentifier
    {
        public ObservableCollection<GameEvent> GameEvents { get; set; } = new ObservableCollection<GameEvent>();
        
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public string Id { get; set; }
        public DateTime StartTime { get; set; }

        public Match()
        {
            Id = Guid.NewGuid().ToString();
            StartTime = DateTime.UtcNow;
        }

        public Match(Player p1, Player p2)
        {
            Player1 = p1;
            Player2 = p2;
            Id = Guid.NewGuid().ToString();
        }        
    }
}
