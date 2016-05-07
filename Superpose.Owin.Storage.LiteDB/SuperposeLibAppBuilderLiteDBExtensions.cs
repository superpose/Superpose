using Owin;
using Superpose.Storage.LiteDB;
using SuperposeLib.Owin;

namespace Superpose.Owin.Storage.LiteDB
{
    public static class SuperposeLibAppBuilderLiteDbExtensions
    {
        public static IAppBuilder UseSuperposeLibLiteDbStorageFactory(this IAppBuilder app)
        {
            SuperposeLibServerMiddleware.StorageFactory = new LiteDBJobStoragefactory();
            return app;
        }
    }
}