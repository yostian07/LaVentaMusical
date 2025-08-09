using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LaVentaMusical.Startup))]
namespace LaVentaMusical
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
