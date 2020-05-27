namespace Fluxor.Blazor.Web.PersistStore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Fluxor.Blazor.Web.PersistStore.Abstractions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;

    public class PersistStoreSessionCleanupService : IHostedService
    {
        private readonly IFluxorStorage storage;

        private readonly IOptions<PersistStoreMiddlewareOptions> options;

        private Timer timer;

        private int cleanupRunningFlag = 0;

        private int cleanupIntervalSeconds;

        public PersistStoreSessionCleanupService(
            IFluxorStorage storage,
            IOptions<PersistStoreMiddlewareOptions> options)
        {
            this.storage = storage;
            this.options = options;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            int keepAlivesSeconds = this.options.Value.SessionKeepAliveIntervalSeconds * 2;
            this.cleanupIntervalSeconds = keepAlivesSeconds + (keepAlivesSeconds / 2);

            this.timer = new Timer(
                this.CleanupSessions,
                null,
                TimeSpan.FromSeconds((double)this.cleanupIntervalSeconds),
                TimeSpan.FromSeconds((double)this.cleanupIntervalSeconds));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.timer != null)
            {
                Interlocked.Exchange(ref this.cleanupRunningFlag, 1);
                try
                {
                    using (ManualResetEvent waitHandle = new ManualResetEvent(false))
                    {
                        if (this.timer.Dispose(waitHandle))
                        {
                            if (!waitHandle.WaitOne(10 * 1000))
                            {
                                throw new TimeoutException(
                                    "Timeout waiting for fluxor persist store session cleanup timer to stop.");
                            }

                            this.timer = null;
                        }
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref this.cleanupRunningFlag, 0);
                }
            }

            return Task.CompletedTask;
        }

        private void CleanupSessions(Object state)
        {
            if (Interlocked.Exchange(ref this.cleanupRunningFlag, 1) == 0)
            {
                try
                {
                    this.storage.PurgeOrphanedStates(this.cleanupIntervalSeconds);
                }
                finally
                {
                    Interlocked.Exchange(ref this.cleanupRunningFlag, 0);
                }
            }
        }
    }
}