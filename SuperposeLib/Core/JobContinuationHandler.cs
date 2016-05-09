using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Serialize.Linq.Serializers;
using Superpose.StorageInterface;
using SuperposeLib.Core.Jobs;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Core
{
    public class JobContinuationHandler
    {
        public string EnqueueJob<T>(Func<JobContinuationHandler, string> continuation = null) where T : AJob
        {
            return EnqueueJob<T>(new DefaultJobQueue(), continuation);
        }

        public string EnqueueJob<T, TCommand>(TCommand command, Func<JobContinuationHandler, string> continuation = null)
            where T : AJob<TCommand> where TCommand : AJobCommand
        {
            return EnqueueJob<T, TCommand>(command, new DefaultJobQueue(), continuation);
        }


        public string EnqueueJob<T>(JobQueue queue, Func<JobContinuationHandler, string> continuation = null)
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
            return EnqueueJob<T>(queue, fun);
        }

        public string EnqueueJob(Func<JobContinuationHandler, List<string>> continuation)
        {
            return EnqueueJob<PilotJob>(new DefaultJobQueue(), continuation);
        }

        public string EnqueueJob(JobQueue queue, Func<JobContinuationHandler, List<string>> continuation)
        {
            return EnqueueJob<PilotJob>(queue, continuation);
        }

        public string EnqueueJob<T>(JobQueue queue, Func<JobContinuationHandler, List<string>> continuation )
            where T : AJob
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage())
            {
                var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();
                IJobFactory factory = new JobFactory(storage, converter);
                return
                    factory.PrepareScheduleJob(typeof (T), null, null, null,
                        continuation?.Invoke(new JobContinuationHandler())).JobLoadString;
            }
        }


        public string EnqueueJob<T, TCommand>(TCommand command, JobQueue queue,
            Func<JobContinuationHandler, List<string>> continuation = null) where T : AJob<TCommand>
            where TCommand : AJobCommand
        {
            using (var storage = SuperposeGlobalConfiguration.StorageFactory.CreateJobStorage())
            {
                var converter = SuperposeGlobalConfiguration.JobConverterFactory.CretateConverter();
                IJobFactory factory = new JobFactory(storage, converter);
                return
                    factory.PrepareScheduleJob(typeof (T), command, null, queue,
                        continuation?.Invoke(new JobContinuationHandler())).JobLoadString;
            }
        }


        public string EnqueueJob<T, TCommand>(TCommand command, JobQueue queue,
            Func<JobContinuationHandler, string> continuation = null) where T : AJob<TCommand>
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
            return EnqueueJob<T, TCommand>(command, queue, fun);
        }


        protected string EnqueueJob_(JobQueue queue, Expression operation,
            Func<JobContinuationHandler, string> continuation = null)
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
                    fun);
        }



        public  string EnqueueJob(Expression<Action> operation,
           Func<JobContinuationHandler, List<string>> continuation)
        {
            return EnqueueJob_(null, operation, continuation);
        }

        public  string EnqueueJob<T>(Expression<Action<T>> operation,
            Func<JobContinuationHandler, List<string>> continuation)
        {
            return EnqueueJob_(null, operation, continuation);
        }

        public  string EnqueueJob<T>(JobQueue queue, Expression<Action<T>> operation,
            Func<JobContinuationHandler, List<string>> continuation)
        {
            return EnqueueJob_(queue, operation, continuation);
        }

        public  string EnqueueJob(JobQueue queue, Expression<Action> operation,
            Func<JobContinuationHandler, List<string>> continuation)
        {
            return EnqueueJob_(queue, operation, continuation);
        }


        public  string EnqueueJob(Expression<Action> operation,
            Func<JobContinuationHandler, string> continuation = null)
        {
            return EnqueueJob_(null, operation, continuation);
        }

        public  string EnqueueJob<T>(Expression<Action<T>> operation,
            Func<JobContinuationHandler, string> continuation = null)
        {
            return EnqueueJob_(null, operation, continuation);
        }

        public  string EnqueueJob<T>(JobQueue queue, Expression<Action<T>> operation,
            Func<JobContinuationHandler, string> continuation = null)
        {
            return EnqueueJob_(queue, operation, continuation);
        }

        public  string EnqueueJob(JobQueue queue, Expression<Action> operation,
            Func<JobContinuationHandler, string> continuation = null)
        {
            return EnqueueJob_(queue, operation, continuation);
        }











        protected string EnqueueJob_(JobQueue queue, Expression operation,
            Func<JobContinuationHandler, List<string>> continuation)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            var serialized = serializer.SerializeText(operation);

            return
                EnqueueJob<LinqJob, LinqJobCommand>(new LinqJobCommand {ExpressionString = serialized, Context = null},
                    queue,
                    continuation);
        }
    }
}