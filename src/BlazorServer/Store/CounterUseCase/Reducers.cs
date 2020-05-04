namespace BlazorServer.Store.CounterUseCase
{
    using Fluxor;

    public static class Reducers
    {
        [ReducerMethod]
        public static CounterState ReduceIncrementCounterAction(CounterState state, IncrementCounterAction action) =>
            new CounterState(clickCount: state.ClickCount + 1);
    }
}
