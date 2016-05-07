using System;
using System.Threading.Tasks;
using Superpose.Storage.InMemory;
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
                var storageFactory = new InMemoryJobStoragefactory();
                // StorageFactory = new LiteDBJobStoragefactory();
                var converterFactory = new DefaultJobConverterFactory();
                var converter = converterFactory.CretateConverter();
                using (var storage = storageFactory.CreateJobStorage())
                {
                    try
                    {
                        IJobFactory factory = new JobFactory(storage, converter);
                        var jobId = factory.QueueJob(typeof(SampleJob));
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
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