using Microsoft.AspNetCore.Mvc;
using GameDev.RockPaperScissors.React.Server.Websockets;

namespace GameDev.RockPaperScissors.React.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RockPaperScissorsController : ControllerBase
    {


        public RockPaperScissorsController()
        {

        }

        [HttpGet(Name = "GetRps")]
        public async Task<IEnumerable<WeatherForecast>?> Get()
        {

            return null;

        }
    }
}
