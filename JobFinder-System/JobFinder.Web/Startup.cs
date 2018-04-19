using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JobFinder.Web.Startup))]

namespace JobFinder.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }
    }
}
