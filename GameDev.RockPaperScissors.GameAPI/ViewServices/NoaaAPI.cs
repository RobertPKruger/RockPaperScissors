using GameDev.RockPaperScissors.GameAPI.API_Models;
using Newtonsoft.Json;

namespace GameDev.RockPaperScissors.GameAPI.ViewServices
{
    public class NoaaAPI : INoaaAPI
    {
        private readonly ILogger<NoaaAPI> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public NoaaAPI(ILogger<NoaaAPI> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            this._clientFactory = clientFactory;
        }

        public async Task<string> GetLocalWeather(double latitude, double longitude)
        {
            var locationsClient = _clientFactory.CreateClient();
            var forecastClient = _clientFactory.CreateClient();


            // Set a custom User-Agent header
            locationsClient.DefaultRequestHeaders.UserAgent.ParseAdd("SistersGameProgramming/1.0 (rpk@electricstory.com)");

            string url = $"https://api.weather.gov/points/{latitude},{longitude}"; //Sisters: 44.2909, -121.5492

            string result;

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
                result = contentFinal; // Return the raw JSON for demonstration
            }
            catch (HttpRequestException e)
            {
                // Log the exception, return an error message, etc.
                _logger.LogError(e, "Error calling NOAA API");
               throw new Exception("Error calling NOAA API", e);
            }

            return result;  

        }

    }
}
