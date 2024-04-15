using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf.Types;
using Sorted.Weather.Core.Rainfall.Queries;
using Sorted.Weather.Core.Rainfall.Validation;
using Sorted.Weather.Core.Response;
using Sorted.Weather.Core.Services;

namespace Sorted.Weather.Core.Rainfall.Handlers
{
    public class GetRainfallReadingHandler(
        IWeatherService weatherService,
        ILogger<GetRainfallReadingHandler> logger,
        IValidator<GetRainfallReadingsQuery> validator) : IRequestHandler<GetRainfallReadingsQuery, GetRainfallReadingResponse>
    {
        private readonly IWeatherService _weatherService = weatherService;
        private readonly ILogger<GetRainfallReadingHandler> _logger = logger;
        private readonly IValidator<GetRainfallReadingsQuery> _validator = validator;

        public async Task<GetRainfallReadingResponse> Handle(GetRainfallReadingsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogInformation("Invalid GetRainfallReadingsQuery");
                    return new ValidationFailed(validationResult.Errors);
                }

                var limit = int.Parse(request.Count);

                var result = await _weatherService.GetRainfallReadings(request.StationId);
                if (result is null)
                {
                    _logger.LogInformation("No response from Weather Api");
                    return new NotFound();
                }

                if (result.Items.Count == 0)
                {
                    _logger.LogInformation("No readings were found from station {stationId}", request.StationId);
                    return new NotFound();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling GetRainfallReadingsQuery");
                return new Error();
            }
        }

    }
}
