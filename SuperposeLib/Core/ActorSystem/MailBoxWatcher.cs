using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SuperposeLib.Core.ActorSystem
{
    public class MailBoxWatcher<TCommand, TResponse>
    {
        public class MailMessage
        {
            public readonly TCommand Command;
            public readonly TaskCompletionSource<TResponse> TaskSource;
            public readonly Func<TCommand, Task<TResponse>> Work;
            public readonly CancellationToken? CancelToken;

            public MailMessage(
                TCommand command,
                TaskCompletionSource<TResponse> taskSource,
                Func<TCommand, Task<TResponse>> action,
                CancellationToken? cancelToken)
            {
                Command = command;
                TaskSource = taskSource;
                Work = action;
                CancelToken = cancelToken;
            }
        }

        public static async Task Watch(BlockingCollection<MailMessage> queue)
        {
            foreach (var workItem in queue.GetConsumingEnumerable())
            {
                if (workItem.CancelToken.HasValue &&
                    workItem.CancelToken.Value.IsCancellationRequested)
                {
                    workItem.TaskSource.SetCanceled();
                }
                else
                {
                    try
                    {
                        var result = await workItem.Work(workItem.Command);
                        workItem.TaskSource.SetResult(result);
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (ex.CancellationToken == workItem.CancelToken)
                            workItem.TaskSource.SetCanceled();
                        else
                            workItem.TaskSource.SetException(ex);
                    }
                    catch (Exception ex)
                    {
                        workItem.TaskSource.SetException(ex);
                    }
                }
            }
        }
    }
}