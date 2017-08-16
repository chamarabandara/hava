using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HavaApp.Startup))]
namespace HavaApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
