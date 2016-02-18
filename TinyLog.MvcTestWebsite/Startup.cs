using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TinyLog.MvcTestWebsite.Startup))]
namespace TinyLog.MvcTestWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
