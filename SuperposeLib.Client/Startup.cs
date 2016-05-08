using Owin;
using Superpose.Owin.Storage.InMemory;
using SuperposeLib.Owin;

namespace SuperposeLib.Client
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // app.UseSuperposeLiteDbStorage();
            app.UseSuperposeInMemoryStorage();
            app.UseSuperposeServer();
            app.UseSuperposeDashboard();
        }
    }
}