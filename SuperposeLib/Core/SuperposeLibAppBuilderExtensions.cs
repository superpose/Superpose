using Owin;
using SuperposeLib.Services.DefaultConverter;

namespace SuperposeLib.Core
{
    public static class SuperposeLibAppBuilderExtensions
    {
        public static IAppBuilder UseSuperposeLibServerMiddleware(this IAppBuilder app)
        {
            return app.Use<SuperposeLibServerMiddleware>();
        }
      
    }
}