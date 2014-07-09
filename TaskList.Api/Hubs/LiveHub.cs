using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using TaskList.Data;
using TaskList.Model.Models;

namespace TaskList.Api.Hubs
{
    public class LiveHub : Hub
    {
        public static void AvailabilityChanged(int id) 
        {
            /*var repo = new Repository<Availability>(new MisconnexDbContext());
            var avail = repo.Set.Include("Hotel.ServicePricings.Service").Include("Hotel.ContractedRates").Include("RoomType").FirstOrDefault(a => a.Id == id);
            if (avail != null)
            {
                var hub = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
                hub.Clients.All.AvailabilityChanged(avail);
            }*/
        }
    }
}