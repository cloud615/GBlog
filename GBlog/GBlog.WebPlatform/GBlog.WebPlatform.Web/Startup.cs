using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GBlog.WebPlatform.Web.Startup))]
namespace GBlog.WebPlatform.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
