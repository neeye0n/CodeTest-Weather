namespace Sorted.Weather.Core.Services
{
    public interface IWeatherService
    {
        Task<string> GetRainfallReadings(string stationId, int limit = 10);
    }
}
