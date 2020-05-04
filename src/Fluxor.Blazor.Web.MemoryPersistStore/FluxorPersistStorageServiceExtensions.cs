using Fluxor.Blazor.Web.PersistStore.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Fluxor.Blazor.Web.MemoryPersistStore
{
	public static class FluxorPersistStorageServiceExtensions
	{
		public static IServiceCollection AddFluxorPersistStorage(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<IFluxorStorage, MemoryStorage>();
			return serviceCollection;
		}
	}
}
