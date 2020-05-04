namespace Fluxor.Blazor.Web.PersistStore.Actions
{
    public class PersistStoreSetKeyAction : PersistStoreActionBase
    {
        public PersistStoreSetKeyAction (string storeKey)
        {
            this.StoreKey = storeKey;
        }

        public string StoreKey { get; }
    }
}