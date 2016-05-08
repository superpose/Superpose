using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Hosting;

namespace SuperposeLib.Client
{
    public class OwinServer
    {
        public static void StartServer()
        {
            //todo   C:\<Path\To\Solution>\.vs\config\applicationhost.config
            const string baseAddress = "http://*:8008/";
            var options = new StartOptions(baseAddress)
            {
                ServerFactory = "Microsoft.Owin.Host.HttpListener"
            };

            using (WebApp.Start<Startup>(options))
            {
                Console.WriteLine("Server started");
                // Create HttpCient and make a request to api/values : just a test
                var client = new HttpClient();
                var response = client.GetAsync(baseAddress.Replace("*", "localhost") + "api/values").Result;
                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);


                RunAuthenticationSample();

                Console.ReadLine();
            }
        }

        private static void RunAuthenticationSample()
        {
            ConsoleKeyInfo cki;
            do
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Client ready.");
                    Console.WriteLine("Press any key to invoke the service");
                    Console.WriteLine("Press ESC to terminate");
                    var _ac = new AuthenticationContext("https://login.windows.net/contoso7.onmicrosoft.com");
                    AuthenticationResult _arr = null;
                    cki = Console.ReadKey(true);
                    // get the access token
                    _arr = _ac.AcquireToken("https://contoso7.onmicrosoft.com/RichAPI",
                        "be182811-9d0b-45b2-9ffa-52ede2a12230", new Uri("http://whatevah"));
                    // invoke the web API
                    var result = string.Empty;
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                        _arr.AccessToken);
                    var response = httpClient.GetAsync("http://localhost:8008/api/Values/GetProtected").Result;
                    // display the result
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine("==> Successfully invoked the service");
                        Console.WriteLine(result);
                    }
                }
                catch (Exception e)
                {
                    cki = new ConsoleKeyInfo();
                    Console.WriteLine("Error authenticating " + e.Message + " : " + e);
                }
            } while (cki.Key != ConsoleKey.Escape);
        }
    }
}