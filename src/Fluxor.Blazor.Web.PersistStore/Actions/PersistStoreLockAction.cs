namespace Fluxor.Blazor.Web.PersistStore.Actions
{
    public class PersistStoreLockAction : PersistStoreActionBase
    {
        public PersistStoreLockAction (bool locked) {
            this.Locked = locked;
        }
        public bool Locked { get; }
    }
}