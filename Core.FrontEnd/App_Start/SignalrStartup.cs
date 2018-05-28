using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.FrontEnd;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Core.FrontEnd.SignalrStartup))]
namespace Core.FrontEnd
{
    public class SignalrStartup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}