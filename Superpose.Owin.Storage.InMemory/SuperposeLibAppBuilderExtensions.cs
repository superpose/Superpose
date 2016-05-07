using Owin;
using Superpose.Storage.InMemory;
using Superpose.StorageInterface;
using SuperposeLib.Owin;

namespace Superpose.Owin.Storage.InMemory
{
    public static class SuperposeLibAppBuilderExtensions
    {
        public static IAppBuilder UseSuperposeInMemoryStorage(this IAppBuilder app)
        {
            SuperposeGlobalConfiguration.StorageFactory = new InMemoryJobStoragefactory();
            return app;
        }
    }
}