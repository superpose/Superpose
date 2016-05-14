using Owin;
using Superpose.Storage.SqlServer;
using Superpose.StorageInterface;

namespace Superpose.Owin.Storage.SqlServer
{
    public static class SuperposeLibSqlServerAppBuilderExtensions
    {
        public static IAppBuilder UseSuperposeSqlServerStorage(this IAppBuilder app)
        {
            SuperposeGlobalConfiguration.StorageFactory = new SqlServerStoragefactory(SuperposeGlobalConfiguration.StorageFactory.GetCurrentExecutionInstance());
            return app;
        }
    }
}