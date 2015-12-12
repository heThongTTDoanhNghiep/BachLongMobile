using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Web_TMDT.Startup))]
namespace Web_TMDT
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
