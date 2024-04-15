using Microsoft.Extensions.Logging;
using Sorted.Weather.Core.Models;
using System.Text.Json;

namespace Sorted.Weather.Core.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(IHttpClientFactory httpClientFactory,
            ILogger<WeatherService> logger)
        {
            ArgumentNullException.ThrowIfNull(httpClientFactory);
            ArgumentNullException.ThrowIfNull(logger);
            _httpClient = httpClientFactory.CreateClient("WeatherClient");
            _logger = logger;
        }

        public async Task<RainfallApiResponse?> GetRainfallReadings(string stationId, int limit = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"id/stations/{stationId}/readings?_sorted&_limit={limit}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<RainfallApiResponse>(result);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError("Json deserialization error: {jsonEx}", jsonEx);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("HTTP request failed: {ex}", ex);
                return null;
            }
        }
    }
}
