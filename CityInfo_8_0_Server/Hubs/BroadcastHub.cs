using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server.Hubs
{
    public class BroadcastHub : Hub
    {
        public async Task SendCityDataUpdateMessage()
        {
            await Clients.All.SendAsync("UpdateCityDataMessage");
        }
    }
}
