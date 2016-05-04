using Owin;
using SuperposeLib.Services.DefaultConverter;
using SuperposeLib.Services.InMemoryStorage;

namespace SuperposeLib.Core
{
    public static class SuperposeLibAppBuilderExtensions
    {
        public static IAppBuilder UseSuperposeLibServerMiddleware(this IAppBuilder app)
        {
            return app.Use<SuperposeLibServerMiddleware>();
        }
        public static IAppBuilder UseSuperposeLibInMemoryStorageFactory(this IAppBuilder app)
        {
            SuperposeLibServerMiddleware.StorageFactory = new InMemoryJobStoragefactory();
            return app;
        }
    }
}