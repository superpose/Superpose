using Owin;
using SuperposeLib.Owin;
namespace SuperposeLib.Client
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseUIMiddleware();
        }
    }
}