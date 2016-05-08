using SuperposeLib.Core;
using SuperposeLib.Owin;

namespace SuperposeLib.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            OwinServer.StartServer();
        }
    }

    public class SampleJob : AJob
    {
        protected override void Execute()
        {
            SuperposeSignalRContext.GetHubContext().Clients.All.AddMessage("Just Ran!");
        }
    }
}