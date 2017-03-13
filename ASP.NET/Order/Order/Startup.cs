using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Order.Startup))]
namespace Order
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
