using Microsoft.Owin;
using Owin;
using ApaScoreKeeperWeb;

namespace ApaScoreKeeperWeb
{
    public partial class Startup
    {
        public void ConfigureSignalR(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}