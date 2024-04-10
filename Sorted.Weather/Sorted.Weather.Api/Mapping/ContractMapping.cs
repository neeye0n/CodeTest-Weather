using OneOf.Types;
using Sorted.Weather.Api.Contracts;
using Sorted.Weather.Core.Models;
using Sorted.Weather.Core.Rainfall.Validation;

namespace Sorted.Weather.Api.Mapping
{
    public static class ContractMapping
    {
        public static RainfallReadingResponse MapToReponse(this RainfallApiResponse source)
        {
            var rainfallReadingResponse = new RainfallReadingResponse();
            rainfallReadingResponse.Readings = new List<Contracts.RainfallReading>();

            foreach (var src in source.Items)
            {
                rainfallReadingResponse.Readings.Add(new Contracts.RainfallReading() { DateMeasured = src.DateTime, AmountMeasured = src.Value });
            }

            return rainfallReadingResponse;
        }

        public static ErrorResponse MapToReponse(this NotFound source)
        {
            var errorResponse = new ErrorResponse()
            {
                Message = "No readings found for the specified stationId",
            };

            return errorResponse;
        }

        public static ErrorResponse MapToReponse(this ValidationFailed source)
        {
            var errorResponse = new ErrorResponse()
            {
                Message = "Invalid request",
                Items = source.errors.Select(x => new ErrorDetail
                {
                    PropertyName = x.PropertyName,
                    Message = x.ErrorMessage
                }).Distinct().ToList()
            };

            return errorResponse;
        }

        public static ErrorResponse MapToReponse(this Error source)
        {
            var errorResponse = new ErrorResponse()
            {
                Message = "Internal server error",
            };

            return errorResponse;
        }
    }
}
