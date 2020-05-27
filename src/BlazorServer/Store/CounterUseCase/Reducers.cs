namespace BlazorServer.Store.CounterUseCase
{
    using Fluxor;
    using Fluxor.Blazor.Web.PersistStore.DevFluxor;

    public static class Reducers
    {
        [ReducerMethod]
        public static DevFluxorCounterState ReduceIncrementCounterAction(DevFluxorCounterState state, IncrementCounterAction action) =>
            new DevFluxorCounterState(clickCount: state.ClickCount + 1);
    }
}
