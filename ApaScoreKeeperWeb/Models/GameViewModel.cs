namespace ApaScoreKeeperWeb
{
    public class GameViewModel
    {
        public PlayerViewModel Player1 { get; set; }
        public PlayerViewModel Player2 { get; set; }
    }

    public class PlayerViewModel
    {
        public string Name { get; set; }
        public string Team { get; set; }
        public int Points { get; set; }
        public int PointsNeeded { get; set; }

    }
}