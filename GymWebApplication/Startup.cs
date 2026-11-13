using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GymWebApplication.Startup))]
namespace GymWebApplication
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
