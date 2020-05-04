using System;
using System.Collections.Generic;
using BlazorServer.Data;

namespace BlazorServer.Store.WeatherUseCase
{
    public class WeatherState
    {
       	public bool IsLoading { get; }
	    public IEnumerable<WeatherForecast> Forecasts { get; }

        public WeatherState(bool isLoading, IEnumerable<WeatherForecast> forecasts)
        {
            IsLoading = isLoading;
            Forecasts = forecasts ?? Array.Empty<WeatherForecast>();
        }
    }
}