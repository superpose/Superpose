using Owin;
using Superpose.Owin.Storage.InMemory;
using SuperposeLib.Owin;

namespace SuperposeLib.Tests
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseSuperposeInMemoryStorage();
            app.UseSuperposeServer();
        }
    }
}