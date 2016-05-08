using System;
using Superpose.StorageInterface;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Core
{
    public class JobContinuationHandler
    {

        public  string EnqueueJob<T>(Func<JobContinuationHandler, string> continuation = null) where T : AJob
        {
            return EnqueueJob<T>(new DefaultJobQueue(), continuation);
        }

        public  string EnqueueJob<T, TCommand>(TCommand command,
            Func<JobContinuationHandler, string> continuation = null) where T : AJob<TCommand>
            where TCommand : AJobCommand
        {
            return EnqueueJob<T, TCommand>(command, new DefaultJobQueue(), continuation);
        }


        public  string EnqueueJob<T>(JobQueue queue, Func<JobContinuationHandler,string> continuation = null) where T : AJob
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage())
            {
                var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();
                IJobFactory factory = new JobFactory(storage, converter);
                return factory.PrepareScheduleJob(typeof(T),null,null,null, continuation?.Invoke(new JobContinuationHandler())).JobLoadString;
            }
            
        }

        public  string EnqueueJob<T, TCommand>(TCommand command, JobQueue queue, Func<JobContinuationHandler,string> continuation = null) where T : AJob<TCommand> where TCommand : AJobCommand
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage())
            {
                var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();
                IJobFactory factory = new JobFactory(storage, converter);
                return factory.PrepareScheduleJob(typeof(T), command,null,null, continuation?.Invoke(new JobContinuationHandler())).JobLoadString;
                
            }
        }
    }
}