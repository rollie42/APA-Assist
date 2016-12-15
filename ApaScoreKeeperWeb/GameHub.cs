using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace ApaScoreKeeperWeb
{
    public class GameHub : Hub
    {
        public void Send(GameViewModel game)
        {
            var gameSerialized = JsonConvert.SerializeObject(game);
            Clients.All.updateGame(gameSerialized);
        }
    }
}
