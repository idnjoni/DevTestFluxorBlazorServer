using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBreeze;
using Fluxor.Blazor.Web.PersistStore.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Fluxor.Blazor.Web.DBreezePersistStore
{
    public class DBreezeStorage : IFluxorStorage, IDisposable
    {
        private const string STATESTABLE = "SessionState";

        private const string KEEPALIVETABLE = "SessionKeepAlive";

        private readonly DBreezeEngine engine;

        private bool disposedValue;

        public DBreezeStorage(IOptions<DBreezeStorageOptions> options)
        {
            this.engine = new DBreezeEngine(options.Value.DbFolderPath);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
        }

        public Task SaveStateAsync(string key, Dictionary<string, string> states)
        {
            using (var tran = this.engine.GetTransaction())
            {
                tran.SynchronizeTables(STATESTABLE);
                tran.ValuesLazyLoadingIsOn = false;
                string statesJson = JsonConvert.SerializeObject(states);
                tran.Insert<string, string>(STATESTABLE, key, statesJson);
                tran.Commit();
            }

            return Task.CompletedTask;
        }

        public Task<Dictionary<string, string>> LoadStateAsync(string key)
        {
            using (var tran = this.engine.GetTransaction())
            {
                var row = tran.Select<string, string>(STATESTABLE, key);
                if (row.Exists)
                {
                    Dictionary<string, string> states = JsonConvert.DeserializeObject<Dictionary<string, string>>(row.Value);
                    return Task.FromResult<Dictionary<string, string>>(states);
                }
            }

            return Task.FromResult<Dictionary<string, string>>(null);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.engine?.Dispose();
                }

                this.disposedValue = true;
            }
        }

        public Task KeepAliveState(string key)
        {
            using (var tran = this.engine.GetTransaction())
            {
                tran.SynchronizeTables(KEEPALIVETABLE);
                tran.Insert<string, DateTime>(KEEPALIVETABLE, key, DateTime.UtcNow);
                tran.Commit();
            }

            return Task.CompletedTask;
        }

        public Task PurgeOrphanedStates(int timeFrameSeconds)
        {
            DateTime borderDate = DateTime.UtcNow.AddSeconds(timeFrameSeconds * -1);
            using (var tran = this.engine.GetTransaction())
            {
                tran.SynchronizeTables(KEEPALIVETABLE, STATESTABLE);
                foreach (var rowSession in tran.SelectForward<string, DateTime>(KEEPALIVETABLE).Where(r => r.Value < borderDate).ToList())
                {
                    var rowState = tran.Select<string, string>(STATESTABLE, rowSession.Key);
                    if (rowState.Exists)
                    {
                        tran.RemoveKey(STATESTABLE, rowSession.Key);
                    }

                    tran.RemoveKey(KEEPALIVETABLE, rowSession.Key);
                }

                tran.Commit();
            }

            return Task.CompletedTask;
        }
    }
}
