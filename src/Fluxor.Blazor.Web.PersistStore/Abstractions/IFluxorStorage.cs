using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fluxor.Blazor.Web.PersistStore.Abstractions
{
    public interface IFluxorStorage
    {
        Task SaveStateAsync(string key, Dictionary<string, string> states);

        Task<Dictionary<string, string>> LoadStateAsync(string key);

        Task KeepAliveState(string key);
    }
}