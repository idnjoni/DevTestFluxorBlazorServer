using Fluxor.Exceptions;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Fluxor.Blazor.Web.PersistStore.Actions;

namespace Fluxor.Blazor.Web.PersistStore
{
    /// <summary>
    /// Initializes the stoe for the current user. This should be placed in the App.razor component.
    /// </summary>
    public class StoreRestorer : ComponentBase
    {
        [Inject]
        private IDispatcher Dispatcher { get; set; }

        [Inject]
        private SessionStorage SessionStorage { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Dispatcher.Dispatch(new PersistStoreLockAction(true));
        }

        /// <summary>
        /// Executes any supporting JavaScript required for Middleware
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    string sessionKey = await SessionStorage.GetItemAsync("sessionKey");
                    if (string.IsNullOrEmpty(sessionKey))
                    {
                        sessionKey = Guid.NewGuid().ToString();
                        await SessionStorage.SetItemAsync("sessionKey", sessionKey);
                    }

                    Dispatcher.Dispatch(new PersistStoreSetKeyAction(sessionKey));
                }
                catch (TaskCanceledException)
                {
                    // The browser has disconnected from a server-side-blazor app and can no longer be reached.
                    // Swallow this exception as the store will be abandoned and garbage collected.
                    return;
                }
                catch (Exception err)
                {
                    throw new StoreInitializationException("Store restore state error", err);
                }
                finally
                {
                    Dispatcher.Dispatch(new PersistStoreLockAction(false));
                }
            }
        }
    }
}
