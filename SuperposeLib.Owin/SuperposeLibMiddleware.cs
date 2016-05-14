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
        public static string LastReportedProcessedJob;

        public Action<string> UiNotifyer = jobId =>
        {
            LastReportedProcessedJob = jobId;

            if (ReportDelayInProgress)
            {
                return;
            }
            ReportDelayInProgress = true;
            Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(n =>
            {
                SuperposeSignalRContext.GetHubContext().Clients.All.Processing(LastReportedProcessedJob);
                var jobStatistics = Storage.JobLoader.GetJobStatistics();
                SuperposeSignalRContext.GetHubContext().Clients.All.jobStatisticsCompleted(jobStatistics);
                ReportDelayInProgress = false;
            });
        };

        public SuperposeLibServerMiddleware(AppFunc next)
        {
            Next = next;
            SuperposeGlobalConfiguration.JobConverterFactory = SuperposeGlobalConfiguration.JobConverterFactory ??
                                                               new DefaultJobConverterFactory();
            var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();
            Storage = SuperposeGlobalConfiguration.StorageFactory.GetJobStorage(SuperposeGlobalConfiguration.StorageFactory.GetCurrentExecutionInstance());
            Runner = new DefaultJobRunner(Storage, converter);
            Task.Delay(TimeSpan.FromMilliseconds(10)).ContinueWith(c =>
            {
                Runner.Run(UiNotifyer, UiNotifyer);
            });
        }

        public static IJobStorage Storage { get; set; }
        private AppFunc Next { get; }
        private IJobRunner Runner { get; }

        public static bool ReportDelayInProgress { set; get; }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            await Next.Invoke(environment);
        }
    }
}