using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Superpose.StorageInterface;
using SuperposeLib.Core;
using SuperposeLib.Services.DefaultConverter;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace SuperposeLib.Owin
{
    public class SuperposeLibServerMiddleware
    {
        public SuperposeLibServerMiddleware(AppFunc next)
        {
            Next = next;

            var converter = new DefaultJobConverterFactory().CretateConverter();
            var storage = StorageFactory.CreateJobStorage();
            Runner = new JobRunner(storage, converter);
            Runner.Run((jobId) => Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith((n) =>
            {
                SuperposeSignalRContext.GetHubContext().Clients.All.AddMessage(jobId);

            }));
        }

        public static IJobStoragefactory StorageFactory { set; get; }
        private AppFunc Next { get; }
        private IJobRunner Runner { get; }


        public async Task Invoke(IDictionary<string, object> environment)
        {
            await Next.Invoke(environment);
        }
    }
}