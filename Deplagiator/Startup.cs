using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Deplagiator.Startup))]
[assembly: OwinStartup(typeof(Deplagiator.Startup))]
namespace Deplagiator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
