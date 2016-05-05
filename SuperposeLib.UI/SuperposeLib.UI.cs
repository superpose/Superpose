using System;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Security.ActiveDirectory;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

namespace SuperposeLib.UI
{
    public class SetUpOwinThings
    {
        public static void SetUpAuthentication(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Audience = "https://contoso7.onmicrosoft.com/RichAPI",
                    Tenant = "contoso7.onmicrosoft.com"
                });
        }

        public static void SetUpIntegratedWindowsAuthentication(IAppBuilder app)
        {
            var listener =
                (HttpListener) app.Properties["System.Net.HttpListener"];
            listener.AuthenticationSchemes =
                AuthenticationSchemes.IntegratedWindowsAuthentication;
        }

        public static void SetUpFileServer(IAppBuilder app)
        {
            var fileSystem = new PhysicalFileSystem(AppDomain.CurrentDomain.BaseDirectory + "/ui");
            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                FileSystem = fileSystem,
                EnableDefaultFiles = true
            };

            app.UseFileServer(options);
        }

        public static void SetUpWebApi(IAppBuilder app, HttpConfiguration config)
        {
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings =
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional}
                );

            app.UseWebApi(config);
        }
    }
}