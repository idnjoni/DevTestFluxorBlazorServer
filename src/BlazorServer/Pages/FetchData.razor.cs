namespace BlazorServer.Pages
{
    using Fluxor;
    using Microsoft.AspNetCore.Components;
    using BlazorServer.Store.WeatherUseCase;
    using System.Threading.Tasks;

    public partial class FetchData
    {
        [Inject]
	    private IState<WeatherState> WeatherState { get; set; }

        [Inject]
	    private IDispatcher Dispatcher { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            // Dispatcher.Dispatch(new FetchDataAction());
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Dispatcher.Dispatch(new FetchDataAction());
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                Dispatcher.Dispatch(new FetchDataAction());
            }
        }
    }
}