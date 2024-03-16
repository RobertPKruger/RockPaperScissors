namespace GameDev.RockPaperScissors.React.Server
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureF { get; set; }

        //5/9*(F-32)
        public int TemperatureC => (int)(5.0/9.0 * ((double)TemperatureF - 32.0));

        public string? Summary { get; set; }
    }
}
