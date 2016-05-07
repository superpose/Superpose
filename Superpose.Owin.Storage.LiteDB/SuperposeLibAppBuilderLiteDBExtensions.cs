using Owin;
using Superpose.Storage.LiteDB;
using Superpose.StorageInterface;
using SuperposeLib.Owin;

namespace Superpose.Owin.Storage.LiteDB
{
    public static class SuperposeLibAppBuilderLiteDbExtensions
    {
        public static IAppBuilder UseSuperposeLiteDbStorage(this IAppBuilder app)
        {
            SuperposeGlobalConfiguration.StorageFactory = new LiteDBJobStoragefactory();
            return app;
        }
    }
}