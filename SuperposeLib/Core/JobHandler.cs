using System;
using System.Linq.Expressions;
using Serialize.Linq.Serializers;
using Superpose.StorageInterface;
using SuperposeLib.Core.Jobs;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Core
{
    public class JobHandler
    {
        public static string EnqueueJob(Expression<Action> operation,
            Func<JobContinuationHandler, string> continuation = null)
        {
            return EnqueueJob_(null, operation, continuation);
        }

        public static string EnqueueJob<T>(Expression<Action<T>> operation,
            Func<JobContinuationHandler, string> continuation = null)
        {
            return EnqueueJob_(null, operation, continuation);
        }

        public static string EnqueueJob<T>(JobQueue queue, Expression<Action<T>> operation,
            Func<JobContinuationHandler, string> continuation = null)
        {
            return EnqueueJob_(queue,operation, continuation);
        }

        public static string EnqueueJob(JobQueue queue, Expression<Action> operation,
            Func<JobContinuationHandler, string> continuation = null)
        {
            return EnqueueJob_(queue,operation, continuation);
        }

        protected static string EnqueueJob_(JobQueue queue, Expression operation,
            Func<JobContinuationHandler, string> continuation = null)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            var serialized = serializer.SerializeText(operation);

            return
                EnqueueJob<LinqJob, LinqJobCommand>(new LinqJobCommand {ExpressionString = serialized, Context = null},
                    queue,
                    continuation);
        }

        public static string EnqueueJob<T>(Func<JobContinuationHandler, string> continuation = null) where T : AJob
        {
            return EnqueueJob<T>(new DefaultJobQueue(), continuation);
        }

        public static string EnqueueJob<T, TCommand>(TCommand command,
            Func<JobContinuationHandler, string> continuation = null) where T : AJob<TCommand>
            where TCommand : AJobCommand
        {
            return EnqueueJob<T, TCommand>(command, new DefaultJobQueue(), continuation);
        }

        public static string EnqueueJob<T>(JobQueue queue, Func<JobContinuationHandler, string> continuation = null)
            where T : AJob
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage())
            {
                var factory = GetJobFactory(storage);

                var jobId = factory.QueueJob(typeof (T), null, queue, continuation?.Invoke(new JobContinuationHandler()));
                return jobId;
            }
        }

        public static string EnqueueJob<T, TCommand>(TCommand command, JobQueue queue,
            Func<JobContinuationHandler, string> continuation = null) where T : AJob<TCommand>
            where TCommand : AJobCommand
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage())
            {
                var factory = GetJobFactory(storage);
                var jobId = factory.QueueJob(typeof (T), command, queue,
                    continuation?.Invoke(new JobContinuationHandler()));
                return jobId;
            }
        }

        private static IJobFactory GetJobFactory(IJobStorage storage)
        {
            var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();
            IJobFactory factory = new JobFactory(storage, converter);
            return factory;
        }
    }
}