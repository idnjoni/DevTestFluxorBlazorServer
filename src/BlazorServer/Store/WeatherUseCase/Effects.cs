using System;
using System.Threading.Tasks;
using BlazorServer.Data;
using Fluxor;

namespace BlazorServer.Store.WeatherUseCase
{
    public class Effects
    {
        private readonly WeatherForecastService weatherForecastService;

        private readonly IState<WeatherState> weatherState;

        // Can't inject IStore, because while creating the IStore instance, an instance of each discovered effect class is resolved which end in an stack overflow!
        // But injecting of state works!
        public Effects(WeatherForecastService weatherForecastService, IState<WeatherState> weatherState)
        {
            this.weatherState = weatherState;
            this.weatherForecastService = weatherForecastService;
        }

        [EffectMethod]
        public async Task HandleFetchDataAction(FetchDataAction action, IDispatcher dispatcher)
        {
            bool isLoading = this.weatherState.Value.IsLoading;


            var forecasts = await this.weatherForecastService.GetForecastAsync(DateTime.Now);
            dispatcher.Dispatch(new FetchDataResultAction(forecasts));
        }
    }
}