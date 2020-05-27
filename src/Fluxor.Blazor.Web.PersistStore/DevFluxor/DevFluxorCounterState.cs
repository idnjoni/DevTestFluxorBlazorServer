namespace Fluxor.Blazor.Web.PersistStore.DevFluxor
{
    public class DevFluxorCounterState
    {
        public int ClickCount { get; }

        public DevFluxorCounterState(int clickCount)
        {
            ClickCount = clickCount;
        }
    }
}
