using System;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
//using Vigo.Bas.ManagementAgent.Log;

[assembly: OwinStartup(typeof(VigoBAS_Start.Startup))]
namespace VigoBAS_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Logger.Log.Info("Starting VigoBAS-Start portal");
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {                
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/"),
                ReturnUrlParameter = "",
                ExpireTimeSpan = TimeSpan.FromMinutes(20),
                Provider = new CookieAuthenticationProvider()
            });
        }
    }
}
