namespace BlazorServer.Store.CounterUseCase
{
    using Fluxor;
    using Fluxor.Blazor.Web.PersistStore.DevFluxor;

    //public class Feature : Feature<CounterState>
    public class Feature : Feature<DevFluxorCounterState>
    {
        public override string GetName() => "Counter";
        protected override DevFluxorCounterState GetInitialState() =>
            new DevFluxorCounterState(clickCount: 0);
    }
}
