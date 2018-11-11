using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestServer.Core.Helpers
{
    public static class AsyncHelper
    {
        /// <summary>
        /// Runs the task synchronously.
        /// </summary>
        /// <param name="task">The task.</param>
        public static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(
                async _ =>
                {
                    try
                    {
                        await task().ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        synch.InnerException = e;
                        throw;
                    }
                    finally
                    {
                        synch.EndMessageLoop();
                    }
                },
                null);
            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        /// <summary>
        /// Runs the task synchronously with generic types.
        /// </summary>
        /// <typeparam name="T">Expected return type.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>Object of T</returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            T ret = default(T);

            synch.Post(
                async _ =>
                {
                    try
                    {
                        ret = await task().ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        synch.InnerException = e;
                        throw;
                    }
                    finally
                    {
                        synch.EndMessageLoop();
                    }
                },
                null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return ret;
        }

        /// <summary>
        ///  Exclusive Sync context
        /// </summary>
        private class ExclusiveSynchronizationContext : SynchronizationContext, IDisposable
        {
            /// <summary>
            /// The work items waiting
            /// </summary>
            private readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);

            /// <summary>
            /// The items
            /// </summary>
            private readonly Queue<Tuple<SendOrPostCallback, object>> items = new Queue<Tuple<SendOrPostCallback, object>>();

            /// <summary>
            /// The done
            /// </summary>
            private bool done;

            /// <summary>
            /// Gets or sets the inner exception.
            /// </summary>
            /// <value>
            /// The inner exception.
            /// </value>
            public Exception InnerException { get; set; }

            /// <summary>
            /// When overridden in a derived class, dispatches a synchronous message to a synchronization context.
            /// </summary>
            /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback" /> delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            /// <exception cref="System.NotSupportedException">We cannot send to our same thread</exception>
            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            /// <summary>
            /// When overridden in a derived class, dispatches an asynchronous message to a synchronization context.
            /// </summary>
            /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback" /> delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Post(SendOrPostCallback d, object state)
            {
                lock (this.items)
                {
                    this.items.Enqueue(Tuple.Create(d, state));
                }

                this.workItemsWaiting.Set();
            }

            /// <summary>
            /// Ends the message loop.
            /// </summary>
            public void EndMessageLoop()
            {
                this.Post(_ => this.done = true, null);
            }

            /// <summary>
            /// Begins the message loop.
            /// </summary>
            /// <exception cref="AggregateException">AsyncHelpers.Run method threw an exception.</exception>
            public void BeginMessageLoop()
            {
                while (!this.done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (this.items)
                    {
                        if (this.items.Count > 0)
                        {
                            task = this.items.Dequeue();
                        }
                    }

                    if (task != null)
                    {
                        task.Item1(task.Item2);

                        // the method threw an exception
                        if (this.InnerException != null)
                        {
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", this.InnerException);
                        }
                    }
                    else
                    {
                        this.workItemsWaiting.WaitOne();
                    }
                }
            }

            /// <summary>
            /// When overridden in a derived class, creates a copy of the synchronization context.
            /// </summary>
            /// <returns>
            /// A new <see cref="T:System.Threading.SynchronizationContext" /> object.
            /// </returns>
            public override SynchronizationContext CreateCopy()
            {
                return this;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    workItemsWaiting?.Dispose();
                }
            }
        }
    }
}
