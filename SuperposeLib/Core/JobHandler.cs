using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Serialize.Linq.Serializers;
using Superpose.StorageInterface;
using SuperposeLib.Core.Jobs;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Core
{
    public class JobHandler
    {

        public static string Instance { set; get; }

        public static string EnqueueJob(Expression<Action> operation,
            Func<JobContinuationHandler, List<string>> continuation, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return EnqueueJob_(null, operation, continuation,enqueueStrategy);
        }

        public static string EnqueueJob<T>(Expression<Action<T>> operation,
            Func<JobContinuationHandler, List<string>> continuation, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return EnqueueJob_(null, operation, continuation,enqueueStrategy);
        }

        public static string EnqueueJob<T>(JobQueue queue, Expression<Action<T>> operation,
            Func<JobContinuationHandler, List<string>> continuation, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return EnqueueJob_(queue, operation, continuation,enqueueStrategy);
        }

        public static string EnqueueJob(JobQueue queue, Expression<Action> operation,
            Func<JobContinuationHandler, List<string>> continuation, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return EnqueueJob_(queue, operation, continuation,enqueueStrategy);
        }


        public static string EnqueueJob(Expression<Action> operation,
            Func<JobContinuationHandler, string> continuation = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return EnqueueJob_(null, operation, continuation,enqueueStrategy);
        }

        public static string EnqueueJob<T>(Expression<Action<T>> operation,
            Func<JobContinuationHandler, string> continuation = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return EnqueueJob_(null, operation, continuation,enqueueStrategy);
        }

        public static string EnqueueJob<T>(JobQueue queue, Expression<Action<T>> operation,
            Func<JobContinuationHandler, string> continuation = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return EnqueueJob_(queue, operation, continuation,enqueueStrategy);
        }

        public static string EnqueueJob(JobQueue queue, Expression<Action> operation,
            Func<JobContinuationHandler, string> continuation = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return EnqueueJob_(queue, operation, continuation,enqueueStrategy);
        }


        public static string EnqueueJob<T>(Func<JobContinuationHandler, List<string>> continuation, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
            where T : AJob
        {
            return EnqueueJob<T>(new DefaultJobQueue(), continuation,enqueueStrategy);
        }


        public static string EnqueueJob(JobQueue queue,
            Func<JobContinuationHandler, List<string>> continuation, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return EnqueueJob<PilotJob>(queue, continuation,enqueueStrategy);
        }

        public static string EnqueueJob(
            Func<JobContinuationHandler, List<string>> continuation, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            return EnqueueJob<PilotJob>(new DefaultJobQueue(), continuation,enqueueStrategy);
        }


        public static string EnqueueJob<T, TCommand>(TCommand command,
            Func<JobContinuationHandler, List<string>> continuation, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown) where T : AJob<TCommand>
            where TCommand : AJobCommand
        {
            return EnqueueJob<T, TCommand>(command, new DefaultJobQueue(), continuation,enqueueStrategy);
        }


        public static string EnqueueJob<T>(Func<JobContinuationHandler, string> continuation = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown) where T : AJob
        {
            return EnqueueJob<T>(new DefaultJobQueue(), continuation,enqueueStrategy);
        }

        public static string EnqueueJob<T, TCommand>(TCommand command,
            Func<JobContinuationHandler, string> continuation = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown) where T : AJob<TCommand>
            where TCommand : AJobCommand
        {
            return EnqueueJob<T, TCommand>(command, new DefaultJobQueue(), continuation,enqueueStrategy);
        }


        protected static string EnqueueJob_(JobQueue queue, Expression operation,
            Func<JobContinuationHandler, string> continuation = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            var serialized = serializer.SerializeText(operation);


            Func<JobContinuationHandler, List<string>> fun;

            if (continuation == null)
            {
                fun = null;
            }
            else
            {
                fun = j => new List<string> {continuation(j)};
            }

            return
                EnqueueJob<LinqJob, LinqJobCommand>(new LinqJobCommand {ExpressionString = serialized, Context = null},
                    queue,
                    fun,enqueueStrategy);
        }


        protected static string EnqueueJob_(JobQueue queue, Expression operation,
            Func<JobContinuationHandler, List<string>> continuation, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            var serialized = serializer.SerializeText(operation);

            return
                EnqueueJob<LinqJob, LinqJobCommand>(new LinqJobCommand {ExpressionString = serialized, Context = null},
                    queue,
                    continuation,enqueueStrategy);
        }


        public static string EnqueueJob<T>(JobQueue queue, Func<JobContinuationHandler, string> continuation = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
            where T : AJob

        {
            Func<JobContinuationHandler, List<string>> fun;

            if (continuation == null)
            {
                fun = null;
            }
            else
            {
                fun = j => new List<string> {continuation(j)};
            }
            return EnqueueJob<T>(queue, fun,enqueueStrategy);
        }


        public static string EnqueueJob<T>(JobQueue queue,
            Func<JobContinuationHandler, List<string>> continuation, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown)
            where T : AJob
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.GetJobStorage(Instance))
            {
                var factory = GetJobFactory(storage);

                var jobId = factory.QueueJob(typeof (T), null, queue,
                    continuation?.Invoke(new JobContinuationHandler(Instance)),enqueueStrategy);
                return jobId;
            }
        }

        public static string EnqueueJob<T, TCommand>(TCommand command, JobQueue queue,
            Func<JobContinuationHandler, string> continuation = null, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown) where T : AJob<TCommand>
            where TCommand : AJobCommand
        {
            Func<JobContinuationHandler, List<string>> fun;

            if (continuation == null)
            {
                fun = null;
            }
            else
            {
                fun = j => new List<string> {continuation(j)};
            }
            return EnqueueJob<T, TCommand>(command, queue, fun,enqueueStrategy);
        }


        public static string EnqueueJob<T, TCommand>(TCommand command, JobQueue queue,
            Func<JobContinuationHandler, List<string>> continuation, EnqueueStrategy enqueueStrategy = EnqueueStrategy.Unknown) where T : AJob<TCommand>
            where TCommand : AJobCommand
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.GetJobStorage(Instance))
            {
                var factory = GetJobFactory(storage);
                var jobId = factory.QueueJob(typeof (T), command, queue,
                    continuation?.Invoke(new JobContinuationHandler(Instance)),enqueueStrategy);
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