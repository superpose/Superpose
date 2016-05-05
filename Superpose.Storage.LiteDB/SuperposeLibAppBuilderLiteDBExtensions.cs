using Owin;
using SuperposeLib.Core;

namespace Superpose.Storage.LiteDB
{
    public static class SuperposeLibAppBuilderLiteDBExtensions
    {
        
        public static IAppBuilder UseSuperposeLibInMemoryStorageFactory(this IAppBuilder app)
        {
            SuperposeLibServerMiddleware.StorageFactory = new LiteDBJobStoragefactory();
            return app;
        }
    }
}