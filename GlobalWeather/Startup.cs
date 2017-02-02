using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GlobalWeather.Startup))]
namespace GlobalWeather
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
