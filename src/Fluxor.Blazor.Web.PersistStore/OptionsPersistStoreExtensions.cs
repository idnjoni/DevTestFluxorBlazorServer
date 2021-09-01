using System;
using Fluxor.Blazor.Web.PersistStore.Interop;
using Fluxor.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Fluxor.Blazor.Web.PersistStore
{
	public static class OptionsPersistStoreExtensions
	{
		public static FluxorOptions UsePersistStore(this FluxorOptions options, Action<PersistStoreMiddlewareOptions> configure = null)
		{
			Action<PersistStoreMiddlewareOptions> configAction = configure;
            if (configure == null)
            {
                configAction = (config) => {};
            }

			options.Services.AddScoped<FluxorPersistStoreInterop>();
            options.Services.Configure(configAction);
            options.Services.AddHostedService<PersistStoreSessionCleanupService>();
            options.AddMiddleware<PersistStoreMiddleware>();
			return options;
		}
	}
}
