using Owin;
using Superpose.Storage.InMemory;
using SuperposeLib.Core;

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