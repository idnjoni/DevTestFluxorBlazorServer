namespace BlazorServer.Pages
{
    using Fluxor;
    using Microsoft.AspNetCore.Components;
    using BlazorServer.Store.CounterUseCase;

    public partial class Counter
    {
        [Inject]
        public IState<CounterState> CounterState { get; set; }

        [Parameter]
        public int IncrementAmount { get; set; } = 1;

        [Inject]
    	public IDispatcher Dispatcher { get; set; }

        private void IncrementCount()
        {
            var action = new IncrementCounterAction();
            Dispatcher.Dispatch(action);
        }
    }
}
