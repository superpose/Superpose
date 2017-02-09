using Owin;
using Superpose.Owin.Storage.InMemory;
using Superpose.Owin.Storage.LiteDB;
using Superpose.Owin.Storage.SqlServer;
using SuperposeLib.Owin;

namespace SuperposeLib.Client
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           app.UseSuperposeSqlServerStorage();
             //app.UseSuperposeLiteDbStorage();
            //app.UseSuperposeInMemoryStorage();
            app.UseSuperposeServer();
            app.UseSuperposeDashboard();
        }
    }
}