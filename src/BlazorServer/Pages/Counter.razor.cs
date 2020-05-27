namespace BlazorServer.Pages
{
    using Fluxor;
    using Microsoft.AspNetCore.Components;
    using BlazorServer.Store.CounterUseCase;
    using Fluxor.Blazor.Web.PersistStore.DevFluxor;
    using Fluxor.Blazor.Web.PersistStore.Actions;

    public partial class Counter
    {
        [Inject]
        public IState<DevFluxorCounterState> CounterState { get; set; }

        [Parameter]
        public int IncrementAmount { get; set; } = 1;

        [Inject]
    	public IDispatcher Dispatcher { get; set; }

        private void IncrementCount()
        {
            var action = new IncrementCounterAction();
            Dispatcher.Dispatch(action);
        }

        private void TestResetState()
        {
            var action = new PersistStoreResetStateAction();
            Dispatcher.Dispatch(action);
        }
    }
}
