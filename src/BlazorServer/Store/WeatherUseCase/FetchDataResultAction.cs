using System.Collections.Generic;
using BlazorServer.Data;

namespace BlazorServer.Store.WeatherUseCase
{
    public class FetchDataResultAction
    {
       	public IEnumerable<WeatherForecast> Forecasts { get; }

        public FetchDataResultAction(IEnumerable<WeatherForecast> forecasts)
        {
            Forecasts = forecasts;
        }
    }
}