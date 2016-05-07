using System.Web.Http;
using Microsoft.Owin.Cors;
using Owin;

namespace SuperposeLib.Owin
{
    public static class AppBuilderUiExtensions
    {
        public static IAppBuilder UseSuperposeDashboard(this IAppBuilder app)
        {
           // app.UseCors(CorsOptions.AllowAll);
           // app.UsePacketTrackingMiddleware();
            var config = new HttpConfiguration();

            //TODO FOUND secure entire site globally, does not work with authize attribute
            //COMMENT OUT TO DISABLE THIS FEATURES
            /*
            HTTP/1.1 401 Unauthorized
            Content-Length: 0
            Server: Microsoft-HTTPAPI/2.0
            WWW-Authenticate: Negotiate
            WWW-Authenticate: NTLM
            Date: Sun, 28 Jul 2013 21:02:21 GMT
            Proxy-Support: Session-Based-Authentication
             */
          //  SetUpOwinThings.SetUpIntegratedWindowsAuthentication(app);
          //  SetUpOwinThings.SetUpAuthentication(app);
            SetUpOwinThings.SetUpWebApi(app, config);
            SetUpOwinThings.SetUpFileServer(app);
            app.MapSignalR();

            return app;
        }
    }
}