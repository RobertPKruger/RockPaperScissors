namespace GameDev.RockPaperScissors.GameAPI.ViewServices
{
    public interface INoaaAPI
    {
        Task<string> GetLocalWeather(double latitude, double longitude);
    }
}