using System;
using System.Threading;
using System.Web.Hosting;
using Microsoft.AspNet.SignalR;

namespace AkkaStats.Api.Hubs
{
    public class BackgroundTicker : IRegisteredObject
    {

        private Timer taskTimer;
        private IHubContext hub;
        private int valueOut = 0;
        private Random randomValue;

        public BackgroundTicker()
        {
            HostingEnvironment.RegisterObject(this);
            hub = GlobalHost.ConnectionManager.GetHubContext<StatsHub>();
            taskTimer = new Timer(OnTimerElasped, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        private void OnTimerElasped(object sender)
        {
            hub.Clients.All.broadcastMessage(DateTime.UtcNow.ToString(), valueOut);
            valueOut++;
        }

        public void Stop(bool immediate)
        {
            taskTimer.Dispose();
            HostingEnvironment.UnregisterObject(this);
        }
    }

    public class StatsHub : Hub
    {
      


    }
}