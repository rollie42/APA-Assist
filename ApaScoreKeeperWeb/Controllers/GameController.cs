using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApaScoreKeeperWeb.Controllers
{
    public class GameController : ApiController
    {
        // GET api/values
        public async Task<string> Get()
        {
            var gameHub = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            while (true)
            {
                var model = this.GetRandomGameModel();
                var gameSerialized = JsonConvert.SerializeObject(model);
                gameHub.Clients.All.updateGame(model);
                await Task.Delay(5000);
            }
            return "blah";
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        private GameViewModel GetRandomGameModel()
        {
            var model = new GameViewModel();
            model.Player1 = new PlayerViewModel
            {
                Name = "Player1",
                Team = "Team Joker",
                Points = 10 + DateTime.UtcNow.Millisecond % 10,
                PointsNeeded = 38
            };

            model.Player2 = new PlayerViewModel
            {
                Name = "Player2",
                Team = "Team Queen",
                Points = 20 + DateTime.UtcNow.Millisecond % 10,
                PointsNeeded = 45
            };
            return model;
        }
    }
}
