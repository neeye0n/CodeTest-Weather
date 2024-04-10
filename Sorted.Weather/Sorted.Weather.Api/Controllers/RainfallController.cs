using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sorted.Weather.Api.Contracts;
using Sorted.Weather.Api.Mapping;
using Sorted.Weather.Core.Rainfall.Queries;
using Swashbuckle.AspNetCore.Annotations;

namespace Sorted.Weather.Api.Controllers
{
    /// <summary>
    /// Operations relating to rainfall
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [SwaggerTag("Rainfall", "Operations relating to rainfall")]
    [Produces("application/json")]
    public class RainfallController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Retrieves weather data by station ID.
        /// </summary>
        /// <param name="stationId">The id of the reading station</param>
        /// <param name="count">The number of readings to return. Between 1 - 100</param>
        /// <returns>The weather data for the specified station.</returns>
        [HttpGet("id/{stationId}/readings")]
        [ProducesResponseType(typeof(RainfallReadingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            OperationId = "get-rainfall", 
            Summary = "Get rainfall readings by station Id", 
            Description = "Retrieve the latest readings for the specified stationId", 
            Tags = ["Rainfall"])]
        [SwaggerResponse(StatusCodes.Status200OK, "Get rainfall readings response", typeof(RainfallReadingResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "An error object returned for failed requests", typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "An error object returned for failed requests", typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error object returned for failed requests", typeof(ErrorResponse))]
        public async Task<IActionResult> GetByStationId(string stationId,
            [FromQuery(Name = "count")] string? count)
        {
            var query = string.IsNullOrEmpty(count) ?
                new GetRainfallReadingsQuery(stationId) :
                new GetRainfallReadingsQuery(stationId, count);

            var result = await _mediator.Send(query);

            return result.Match<IActionResult>(
                success => Ok(success.MapToReponse()),
                empty => NotFound(empty.MapToReponse()),
                invalid => BadRequest(invalid.MapToReponse()),
                error => StatusCode(500, error.MapToReponse()));

        }
    }
}
