
using GameDev.RockPaperScissors.GameAPI.ViewServices;
using Microsoft.AspNetCore.Mvc;

namespace GameDev.RockPaperScissors.GameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastLocalController : ControllerBase
    {
        private readonly ILogger<WeatherForecastLocalController> _logger;
        private readonly INoaaAPI noaaAPI;

        public WeatherForecastLocalController(ILogger<WeatherForecastLocalController> logger, INoaaAPI noaaAPI)
        {
            _logger = logger;
            this.noaaAPI = noaaAPI;
        }

        [HttpGet(Name = "GetWeatherForecastLocal")]
        public async Task<IActionResult> GetLocal(double latitude, double longitude)
        {
            try { 
                var noaaResult = await noaaAPI.GetLocalWeather(latitude, longitude);

                // Assuming you have a model class that matches the structure of the response
                // You can deserialize the JSON content into that model
                // For simplicity, we're returning the raw JSON string here
                return Ok(noaaResult); // Return the raw JSON for demonstration
            }
            catch (HttpRequestException e)
            {
                // Log the exception, return an error message, etc.
                _logger.LogError(e, "Error calling NOAA API");
                return StatusCode(500, string.Format("Error calling NOAA API: {0}", e.Message));
            }
        }
    }
}
