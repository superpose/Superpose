using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SuperposeLib.Core.ActorSystem
{
    public abstract class ASlimActor<TCommand, TResponse> : IDisposable
    {
        private readonly BlockingCollection<MailBoxWatcher<TCommand, TResponse>.MailMessage> _mailBox = new BlockingCollection<MailBoxWatcher<TCommand, TResponse>.MailMessage>();

        protected ASlimActor(int workerCount)
        {
            for (var i = 0; i < workerCount; i++)
                Task.Run(async () => await Consume());
        }

        public async Task<bool> Tell(TCommand command
            , Func<TCommand, Task<TResponse>> work
            , CancellationToken? cancelToken
            )
        {
            var tcs = new TaskCompletionSource<TResponse>(command);
            _mailBox.Add(new MailBoxWatcher<TCommand, TResponse>.MailMessage(command, tcs, work, cancelToken));
            return await Task.FromResult(true);
        }

        public async Task<TResponse> Ask(TCommand command
          , Func<TCommand, Task<TResponse>> work
          , CancellationToken? cancelToken
          )
        {
            var tcs = new TaskCompletionSource<TResponse>(command);
            _mailBox.Add(new MailBoxWatcher<TCommand, TResponse>.MailMessage(command, tcs, work, cancelToken));
            return await tcs.Task;
        }

        private async Task Consume()
        {
            await MailBoxWatcher<TCommand, TResponse>.Watch(_mailBox);
        }

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _mailBox.CompleteAdding();
            }
            _disposed = true;
        }

        ~ASlimActor()
        {
            Dispose(false);
        }

        #endregion
    }
}
