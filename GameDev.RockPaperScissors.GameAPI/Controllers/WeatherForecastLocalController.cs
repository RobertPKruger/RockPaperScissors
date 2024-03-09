using GameDev.RockPaperScissors.GameAPI.API_Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static GameDev.RockPaperScissors.GameAPI.API_Models.NoaaLocations;

namespace GameDev.RockPaperScissors.GameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastLocalController : ControllerBase
    {
        private readonly ILogger<WeatherForecastLocalController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public WeatherForecastLocalController(ILogger<WeatherForecastLocalController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            this._clientFactory = clientFactory;
        }

        [HttpGet(Name = "GetWeatherForecastLocal")]
        public async Task<IActionResult> GetLocal(double latitude, double longitude)
        {
            var locationsClient = _clientFactory.CreateClient();
            var forecastClient = _clientFactory.CreateClient();


            // Set a custom User-Agent header
            locationsClient.DefaultRequestHeaders.UserAgent.ParseAdd("SistersGameProgramming/1.0 (rpk@electricstory.com)");

            string url = $"https://api.weather.gov/points/{latitude},{longitude}"; //Sisters: 44.2909, -121.5492

            try
            {
                var response = await locationsClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws an exception if the HTTP response status is an error code.
                var content = await response.Content.ReadAsStringAsync();

                var locations = JsonConvert.DeserializeObject<NoaaLocations>(content);

                //GET THE FORECAST FROM THE PROPERTIES IN A SECOND API CALL

                // Set a custom User-Agent header
                forecastClient.DefaultRequestHeaders.UserAgent.ParseAdd("SistersGameProgramming/1.0 (rpk@electricstory.com)");

                var forecastUrl = locations?.properties.forecast;

                var responseForecast = await forecastClient.GetAsync(forecastUrl);
                responseForecast.EnsureSuccessStatusCode(); // Throws an exception if the HTTP response status is an error code.
                var contentFinal = await responseForecast.Content.ReadAsStringAsync();


                // Assuming you have a model class that matches the structure of the response
                // You can deserialize the JSON content into that model
                // For simplicity, we're returning the raw JSON string here
                return Ok(contentFinal); // Return the raw JSON for demonstration
            }
            catch (HttpRequestException e)
            {
                // Log the exception, return an error message, etc.
                return StatusCode(500, string.Format("Error calling NOAA API: {0}", e.Message));
            }
        }
    }
}
