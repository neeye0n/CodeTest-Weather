using System.Text.Json.Serialization;

namespace Sorted.Weather.Core.Models
{
    public class RainfallApiResponse
    {
        [JsonPropertyName("items")]
        public List<RainfallReading> Items { get; set; }
    }

    public class RainfallReading
    {
        [JsonPropertyName("dateTime")]
        public string DateTime { get; set; }
        [JsonPropertyName("value")]
        public decimal Value { get; set; }
    }

}
