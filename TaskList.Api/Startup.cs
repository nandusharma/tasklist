using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(TaskList.Api.Startup))]

namespace TaskList.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.Use(typeof(StupidMSIEMiddleware));

            ConfigureAuth(app);
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                map.RunSignalR();
            });
        }
    }
}
