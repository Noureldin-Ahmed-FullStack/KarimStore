using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KarimStore.Startup))]
namespace KarimStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
