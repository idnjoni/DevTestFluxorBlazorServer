namespace Fluxor.Blazor.Web.PersistStore.Interop.CallbackObjects
{
	internal class SessionKeepAlivePayload : BasePayload
	{
        #pragma warning disable IDE1006 // Naming Styles
		public string sessionKey { get; set; }
        #pragma warning restore IDE1006 // Naming Styles
	}
}