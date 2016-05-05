using Owin;
using SuperposeLib.Core;

namespace Superpose.Storage.InMemory
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