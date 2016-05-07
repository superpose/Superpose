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
            SuperposeGlobalConfiguration.JobConverterFactory = SuperposeGlobalConfiguration.JobConverterFactory ??
                                                               new DefaultJobConverterFactory();
           var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();
            var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage();
            Runner = new JobRunner(storage, converter);
            Runner.Run((jobId) => Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith((n) =>
            {
                SuperposeSignalRContext.GetHubContext().Clients.All.AddMessage(jobId);

            }));
        }

        private AppFunc Next { get; }
        private IJobRunner Runner { get; }


        public async Task Invoke(IDictionary<string, object> environment)
        {
            await Next.Invoke(environment);
        }
    }
}