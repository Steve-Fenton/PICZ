using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Picz.Startup))]

namespace Picz
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}