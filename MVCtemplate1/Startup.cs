using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCtemplate1.Startup))]
namespace MVCtemplate1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
