using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OneOf.Types;
using Sorted.Weather.Api.Controllers;
using Sorted.Weather.Core.Models;
using Sorted.Weather.Core.Rainfall.Queries;
using Sorted.Weather.Core.Rainfall.Validation;

namespace Sorted.Weather.Tests.Weather.Api
{
    public class RainfallControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RainfallController _rainfallController;

        public RainfallControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _rainfallController = new RainfallController(_mediatorMock.Object);
        }

        [Theory]
        [InlineData("FFVII-R", "1")]
        public async Task GetRainfallReadings_ReturnsOk(string stationid, string count)
        {
            //Arrange
            var expectedResponse = new RainfallApiResponse()
            {
                Items = new List<RainfallReading>
                {
                    new RainfallReading
                    {
                        DateTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString(),
                        Value = 0.1M
                    }
                }
            };

            _mediatorMock
                .Setup(mediator => mediator.Send(It.IsAny<GetRainfallReadingsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);
            //Act
            var result = await _rainfallController.GetByStationId(stationid, count);

            //Assert
            result.Should().BeOfType<OkObjectResult>();

        }

        [Theory]
        [InlineData("FFVII-R", "1")]
        public async Task GetRainfallReadings_ReturnNotFound(string stationid, string count)
        {
            //Arrange
            _mediatorMock
                .Setup(mediator => mediator.Send(It.IsAny<GetRainfallReadingsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new NotFound());
            //Act
            var result = await _rainfallController.GetByStationId(stationid, count);

            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Theory]
        [InlineData("FFVII-R", "1")]
        public async Task GetRainfallReadings_ReturnBadRequest(string stationid, string count)
        {
            //Arrange
            var expectedError = new ValidationFailure("someproperty", "some error message");
            var validationFailed = new ValidationFailed(new List<ValidationFailure> { expectedError });
            _mediatorMock
                .Setup(mediator => mediator.Send(It.IsAny<GetRainfallReadingsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationFailed);

            //Act
            var result = await _rainfallController.GetByStationId(stationid, count);

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData("FFVII-R")]
        public async Task GetRainfallReadings_ReturnInternalServerError(string stationid)
        {
            //Arrange
            _mediatorMock
                .Setup(mediator => mediator.Send(It.IsAny<GetRainfallReadingsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Error());

            //Act
            var result = await _rainfallController.GetByStationId(stationid, string.Empty);

            //Assert
            result.Should().BeOfType<ObjectResult>();
            var resultStatusCode = ((ObjectResult)result).StatusCode;
            resultStatusCode.Should().Be(500);
        }
    }
}
