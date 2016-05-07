using System;
using System.Threading.Tasks;
using Superpose.Storage.InMemory;
using Superpose.StorageInterface;
using SuperposeLib.Core;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Owin;
using SuperposeLib.Services.DefaultConverter;

namespace SuperposeLib.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task.Delay(TimeSpan.FromSeconds(15)).ContinueWith(n =>
            {
                JobHandler.EnqueueJob<SampleJob>();
            });
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