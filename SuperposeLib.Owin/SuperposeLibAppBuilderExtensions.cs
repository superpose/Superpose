using Owin;

namespace SuperposeLib.Owin
{
    public static class SuperposeLibAppBuilderExtensions
    {
        public static IAppBuilder UseSuperposeServer(this IAppBuilder app)
        {
            return app.Use<SuperposeLibServerMiddleware>();
        }
    }
}