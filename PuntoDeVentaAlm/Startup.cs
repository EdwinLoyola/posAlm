using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PuntoDeVentaAlm.Startup))]
namespace PuntoDeVentaAlm
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
