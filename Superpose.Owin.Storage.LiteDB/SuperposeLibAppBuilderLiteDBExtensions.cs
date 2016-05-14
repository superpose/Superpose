using Owin;
using Superpose.Storage.LiteDB;
using Superpose.StorageInterface;

namespace Superpose.Owin.Storage.LiteDB
{
    public static class SuperposeLibAppBuilderLiteDbExtensions
    {
        public static IAppBuilder UseSuperposeLiteDbStorage(this IAppBuilder app)
        {
            SuperposeGlobalConfiguration.StorageFactory = new LiteDbJobStoragefactory();
            return app;
        }
    }
}