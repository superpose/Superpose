using Owin;
using Superpose.Storage.InMemory;
using SuperposeLib.Owin;

namespace Superpose.Owin.Storage.InMemory
{
    public static class SuperposeLibAppBuilderExtensions
    {
        public static IAppBuilder UseSuperposeLibInMemoryStorageFactory(this IAppBuilder app)
        {
            SuperposeLibServerMiddleware.StorageFactory = new InMemoryJobStoragefactory();
            return app;
        }
    }
}