using System;
using System.Threading;
using System.Web.Hosting;
using AkkaStats.Persistance.Interfaces;
using Microsoft.AspNet.SignalR;

namespace AkkaStats.Api.Hubs
{
    public class BackgroundTicker : IRegisteredObject
    {

        private Timer taskTimer;
        private IHubContext hub;
        private int valueOut = 0;
        private Random randomValue;
        private IHubMessageService _hubMessageService;

        public BackgroundTicker(IHubMessageService hubMessageService)
        {
            _hubMessageService = hubMessageService;
            HostingEnvironment.RegisterObject(this);
            hub = GlobalHost.ConnectionManager.GetHubContext<StatsHub>();
            taskTimer = new Timer(OnTimerElasped, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        }

        private void OnTimerElasped(object sender)
        {
            foreach (var message in _hubMessageService.SavedMessages())
            {
                hub.Clients.All.broadcastMessage(message.TimeStamp, message.Message);
            }
            _hubMessageService.Clear();
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