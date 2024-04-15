using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sorted.Weather.Core.Models;
using Sorted.Weather.Core.Rainfall.Handlers;
using Sorted.Weather.Core.Rainfall.Queries;
using Sorted.Weather.Core.Rainfall.Validation;
using Sorted.Weather.Core.Response;
using Sorted.Weather.Core.Services;
using Sorted.Weather.Tests.Extensions;

namespace Sorted.Weather.Tests.Weather.Core.Rainfall
{
    public class GetRainfallReadingQueryTests
    {
        private readonly Mock<IWeatherService> _weatherService;
        private readonly Mock<ILogger<GetRainfallReadingHandler>> _logger;
        private readonly GetRainfallReadingsValidator _validator;

        private readonly string stationId = "E1234F";
        private readonly GetRainfallReadingHandler _handler;

        public GetRainfallReadingQueryTests()
        {
            _weatherService = new Mock<IWeatherService>();
            _logger = new Mock<ILogger<GetRainfallReadingHandler>>();
            _validator = new GetRainfallReadingsValidator();
            _handler = new GetRainfallReadingHandler(_weatherService.Object, _logger.Object, _validator);
        }

        [Fact]
        public async Task Handler_Should_ReturnRainfallApiResponse_IfSuccess()
        {
            // Arrange
            var query = new GetRainfallReadingsQuery(stationId);

            _weatherService.Setup(x => x.GetRainfallReadings(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new RainfallApiResponse()
                {
                    Items = new List<RainfallReading>()
                    {
                        new() { DateTime = "2024-03-15T07:00:00Z", Value = 0.0M }
                    }
                });


            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            result.Should().BeOfType<GetRainfallReadingResponse>();
            result.IsT0.Should().BeTrue();
            result.AsT0.Items.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("0")]
        [InlineData("101")]
        [InlineData("five")]
        public async Task Handler_Should_ReturnValidationFailed_IfCountIsNotWithinRangeOrIsNotNumeric(string count)
        {
            // Arrange
            var query = new GetRainfallReadingsQuery(stationId, count);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            result.Should().BeOfType<GetRainfallReadingResponse>();
            result.IsT2.Should().BeTrue();
            result.AsT2.Errors.Should().NotBeEmpty();
            _logger.VerifyLog(LogLevel.Information, Times.Exactly(1), "Invalid");
        }

        [Fact]
        public async Task Handler_Should_ReturnNotFound_IfNoReadingsFound()
        {
            // Arrange
            var query = new GetRainfallReadingsQuery(stationId);

            _weatherService.Setup(x => x.GetRainfallReadings(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new RainfallApiResponse()
                {
                    Items = new List<RainfallReading>()
                });

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            result.Should().BeOfType<GetRainfallReadingResponse>();
            result.IsT1.Should().BeTrue();
            _logger.VerifyLog(LogLevel.Information, Times.Exactly(1), "No readings were found");
        }

        [Fact]
        public async Task Handler_Should_ReturnNotFound_IfApiReturnsNull()
        {
            // Arrange
            var query = new GetRainfallReadingsQuery(stationId);

            _weatherService.Setup(x => x.GetRainfallReadings(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((RainfallApiResponse)null);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            result.Should().BeOfType<GetRainfallReadingResponse>();
            result.IsT1.Should().BeTrue();
            _logger.VerifyLog(LogLevel.Information, Times.Exactly(1), "No response");
        }

        [Fact]
        public async Task Handler_Should_ReturnError_OnExceptionThrown()
        {
            // Arrange
            var query = new GetRainfallReadingsQuery(stationId);

            _weatherService.Setup(x => x.GetRainfallReadings(It.IsAny<string>(), It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            result.Should().BeOfType<GetRainfallReadingResponse>();
            result.IsT3.Should().BeTrue();
            _logger.VerifyLog(LogLevel.Error, Times.Exactly(1), "An error occurred.");
        }
    }
}

