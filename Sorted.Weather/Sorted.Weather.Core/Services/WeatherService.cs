namespace Sorted.Weather.Core.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        public WeatherService(IHttpClientFactory httpClientFactory)
        {
            ArgumentNullException.ThrowIfNull(httpClientFactory);
            _httpClient = httpClientFactory.CreateClient("WeatherClient");
        }

        public async Task<string> GetRainfallReadings(string stationId, int limit = 10)
        {
            var response = await _httpClient.GetAsync($"id/stations/{stationId}/readings?_sorted&_limit={limit}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
