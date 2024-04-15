using Sorted.Weather.Core.Models;

namespace Sorted.Weather.Core.Services
{
    public interface IWeatherService
    {
        Task<RainfallApiResponse?> GetRainfallReadings(string stationId, int limit = 10);
    }
}
