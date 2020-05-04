using Fluxor.Blazor.Web.PersistStore.Interop.CallbackObjects;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Fluxor.Blazor.Web.PersistStore.Interop
{

	/// <summary>
	/// Interop for persist store actions
	/// </summary>
	internal sealed class FluxorPersistStoreInterop : IDisposable
	{
		private const string FluxorPersistStoreId = "__FluxorPersistStore__";

		private const string ToJsInitMethodName = "init";

		private const string FromJsSessionKeepAliveActionTypeName = "keepalive";

		private readonly IJSRuntime JSRuntime;

		private readonly DotNetObjectReference<FluxorPersistStoreInterop> dotNetRef;

		private bool IsInitializing;

        private bool disposedValue;

		public const string PersistStoreCallbackId = "FluxorPersistStoreCallback";

		public Func<SessionKeepAliveCallback, Task> OnSessionKeepAlive;

        /// <summary>
        /// Creates an instance of the persist store interop
        /// </summary>
        /// <param name="jsRuntime"></param>
        public FluxorPersistStoreInterop(IJSRuntime jsRuntime)
		{
			JSRuntime = jsRuntime;
			dotNetRef = DotNetObjectReference.Create(this);
		}

		internal async ValueTask InitializeAsync()
		{
			IsInitializing = true;
			try
			{
				await InvokeFluxorPersistStoreMethodAsync<object>(ToJsInitMethodName, dotNetRef);
			}
			finally
			{
				IsInitializing = false;
			}
		}

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(true);
        }

		/// <summary>
		/// Called back from browser persist store scripts
		/// </summary>
		/// <param name="messageAsJson"></param>
		[JSInvokable(PersistStoreCallbackId)]
		public async Task PersistStoreCallback(string messageAsJson)
		{
			if (string.IsNullOrWhiteSpace(messageAsJson))
				return;

			var message = JsonConvert.DeserializeObject<BaseCallbackObject>(messageAsJson);
			switch (message?.payload?.type)
			{
				case FromJsSessionKeepAliveActionTypeName:
					Func<SessionKeepAliveCallback, Task> sessionKeepAlive = OnSessionKeepAlive;
					if (sessionKeepAlive != null)
					{
						var callbackInfo = JsonConvert.DeserializeObject<SessionKeepAliveCallback>(messageAsJson);
						Task task = sessionKeepAlive(callbackInfo);
						if (task != null)
							await task;
					}

					break;
			}
		}

        private void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.dotNetRef.Dispose();
                }

                this.disposedValue = true;
            }
        }

		private ValueTask<TResult> InvokeFluxorPersistStoreMethodAsync<TResult>(string identifier, params object[] args)
		{
			if (!IsInitializing)
            {
				return new ValueTask<TResult>(default(TResult));
            }

			string fullIdentifier = $"{FluxorPersistStoreId}.{identifier}";
			return JSRuntime.InvokeAsync<TResult>(fullIdentifier, args);
		}

		internal static string GetClientScripts()
		{
			return $@"
window.{FluxorPersistStoreId} = new (function() {{
    this.SendKeepAlive = function() {{
        let key = window.sessionStorage.getItem('sessionKey');
        if (key) {{
            // Notify Fluxor Persist Store of the presence of the session key
            const detectedMessage = {{
                payload: {{
                    type: '{FromJsSessionKeepAliveActionTypeName}',
                    sessionKey: key
                }}
            }};
            const detectedMessageAsJson = JSON.stringify(detectedMessage);
            window.fluxorDevToolsDotNetInterop.invokeMethodAsync('{PersistStoreCallbackId}', detectedMessageAsJson);
        }}
    }}

    this.{ToJsInitMethodName} = function(dotNetCallbacks, state) {{
        window.fluxorDevToolsDotNetInterop = dotNetCallbacks;
        if (window.fluxorDevToolsDotNetInterop) {{
            window.setInterval(this.SendKeepAlive, 5 * 1000 * 60);
        }}
    }};

}})();
";
        }
    }
}
