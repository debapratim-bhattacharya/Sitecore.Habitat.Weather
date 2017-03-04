using Sitecore.Feature.Weather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Feature.Weather.Repositories
{
    interface IWeatherRepository
    {
         string GetWeatherByCity(WeatherRequestModel request);
        string GetWeatherForecastByCity(WeatherRequestModel request);

    }
}
