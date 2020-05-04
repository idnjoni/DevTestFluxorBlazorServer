using Fluxor.Blazor.Web.PersistStore.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fluxor.Blazor.Web.DBreezePersistStore
{
	public static class FluxorPersistStorageServiceExtensions
	{
		public static IServiceCollection AddFluxorPersistStorage(this IServiceCollection serviceCollection, IConfiguration configuration)
		{
            serviceCollection.Configure<DBreezeStorageOptions>(options => configuration.GetSection("FluxorStorageDBreeze").Bind(options));
            serviceCollection.AddSingleton<IFluxorStorage, DBreezeStorage>();
        	return serviceCollection;
		}
	}
}
