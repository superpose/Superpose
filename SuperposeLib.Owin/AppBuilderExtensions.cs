using Owin;

namespace SuperposeLib.Owin
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UsePacketTrackingMiddleware(
            this IAppBuilder app)
        {
            return app.Use<PacketTrackingMiddleware>();
        }
    }
}