using Owin;
using Superpose.Owin.Storage.InMemory;
using Superpose.Storage.InMemory;
using SuperposeLib.Core;
using SuperposeLib.Owin;

namespace SuperposeLib.Tests
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseSuperposeLibInMemoryStorageFactory();
            app.UseSuperposeLibServerMiddleware();
        }
    }
}