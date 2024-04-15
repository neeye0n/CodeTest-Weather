namespace Sorted.Weather.Api.Contracts
{
    public class RainfallReadingResponse
    {
        public List<RainfallReading>? Readings { get; set; }
    }

    public class RainfallReading
    {
        public string? DateMeasured { get; set; }
        public decimal AmountMeasured { get; set; }
    }
}
