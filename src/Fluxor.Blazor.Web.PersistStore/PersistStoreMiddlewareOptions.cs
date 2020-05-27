using System;
using System.Collections.Generic;

namespace Fluxor.Blazor.Web.PersistStore
{
    public class PersistStoreMiddlewareOptions
    {
        public PersistStoreMiddlewareOptions()
        {
            this.IgnoredFeatures = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "@routing"
            };
        }

        public HashSet<string> IgnoredFeatures { get; }

        public bool IgnoreInitialDispatchesOnRestore { get; set; } = true;

        public int SessionKeepAliveIntervalSeconds { get; set; } = 5 * 60;
    }
}