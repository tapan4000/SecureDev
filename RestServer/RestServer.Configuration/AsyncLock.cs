using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestServer.Configuration
{
    public sealed class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private readonly Task<IDisposable> releaser;

        public AsyncLock()
        {
            this.releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var wait = semaphore.WaitAsync();
            return wait.IsCompleted ? releaser : wait.ContinueWith(
                (_, state) => (IDisposable)state, 
                this.releaser.Result, 
                CancellationToken.None, 
                TaskContinuationOptions.ExecuteSynchronously, 
                TaskScheduler.Default);
        }

        public void Dispose()
        {
            if(null != this.semaphore)
            {
                this.semaphore.Dispose();
            }
        }

        private sealed class Releaser: IDisposable
        {
            private readonly AsyncLock toRelease;
            internal Releaser(AsyncLock toRelease) { this.toRelease = toRelease; }
            public void Dispose()
            {
                toRelease.semaphore.Release();
            }
        }
    }
}
