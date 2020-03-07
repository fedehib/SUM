using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SUM.Startup))]
namespace SUM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
