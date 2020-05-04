namespace BlazorServer.Store.CounterUseCase
{
    using Fluxor;

    public class Feature : Feature<CounterState>
    {
        public override string GetName() => "Counter";
        protected override CounterState GetInitialState() =>
            new CounterState(clickCount: 0);
    }
}
