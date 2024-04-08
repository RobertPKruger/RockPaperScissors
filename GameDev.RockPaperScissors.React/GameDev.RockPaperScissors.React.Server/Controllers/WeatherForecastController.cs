using Microsoft.AspNetCore.Mvc;
using GameDev.RockPaperScissors.GameAPI.ViewServices;
using Newtonsoft.Json;
using GameDev.RockPaperScissors.React.Server.Models;

namespace GameDev.RockPaperScissors.React.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly INoaaAPI noaaAPI;

        public WeatherForecastController(INoaaAPI noaaAPI)
        {
            this.noaaAPI = noaaAPI;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>?> Get()
        {
            var localWeatherString = await noaaAPI.GetLocalWeather(44.1, -121.12);

            var localWeather = JsonConvert.DeserializeObject<NoaaForecast>(localWeatherString);

            var forecastArray = localWeather?.properties.periods?.Select(period => new WeatherForecast
            {
                Date = new DateOnly(period.startTime.Year, period.startTime.Month, period.startTime.Day),
                TemperatureF = period.temperature,
                Summary = period.detailedForecast
            }).ToArray();

            return forecastArray;

        }
    }
}
