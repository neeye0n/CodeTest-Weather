namespace Sorted.Weather.Api.Contracts
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public List<object> Detail { get; set; }
        public List<ErrorDetail> Items { get; set; }
    }

    public class ErrorDetail 
    {
        public string PropertyName { get; set; }
        public string Message { get; set; }
    }
}
