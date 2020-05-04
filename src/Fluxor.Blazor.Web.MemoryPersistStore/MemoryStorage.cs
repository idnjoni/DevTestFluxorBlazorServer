using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fluxor.Blazor.Web.PersistStore.Abstractions;

namespace Fluxor.Blazor.Web.MemoryPersistStore
{
    public class MemoryStorage : IFluxorStorage
    {
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> stateStore;

        private readonly ConcurrentDictionary<string, DateTime> keepAliveStore;

        public MemoryStorage()
        {
            this.stateStore = new ConcurrentDictionary<string, Dictionary<string, string>>();
            this.keepAliveStore = new ConcurrentDictionary<string, DateTime>();
        }

        public Task SaveStateAsync(string key, Dictionary<string, string> states)
        {
            this.stateStore[key] = states;

            return Task.CompletedTask;
        }

        public Task<Dictionary<string, string>> LoadStateAsync(string key)
        {
            if (this.stateStore.TryGetValue(key, out Dictionary<string, string> states))
            {
                return Task.FromResult<Dictionary<string, string>>(states);
            }

            return Task.FromResult<Dictionary<string, string>>(null);
        }

        public Task KeepAliveState(string key)
        {
            if (this.stateStore.Keys.Contains(key))
            {
                this.keepAliveStore[key] = DateTime.UtcNow;
            }

            return Task.CompletedTask;
        }
    }
}
