using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Sorted.Weather.Core.Models;
using Sorted.Weather.Core.Services;
using Sorted.Weather.Tests.Extensions;
using System.Net;

namespace Sorted.Weather.Tests.Weather.Core.Services
{
    public class WeatherServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<ILogger<WeatherService>> _logger;
        private readonly string stationId = "E1234F";
        private readonly WeatherService _weatherService;

        public WeatherServiceTests()
        {
            _logger = new Mock<ILogger<WeatherService>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://example.com/")
            };

            _httpClientFactoryMock.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            _weatherService = new WeatherService(_httpClientFactoryMock.Object, _logger.Object);
        }

        [Fact]
        public async Task GetRainfallReadings_Should_ReturnReadings_IfSuccessful()
        {
            // Arrange
            var expectedResponseString = @" { ""@context"": """", ""meta"": {}, ""items"": [] } ";

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedResponseString)
                });

            // Act
            var response = await _weatherService.GetRainfallReadings(stationId);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<RainfallApiResponse>();
        }

        [Fact]
        public async Task GetRainfallReadings_Should_ReturnNull_OnEmptyResponse()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(string.Empty)
                });

            // Act
            var response = await _weatherService.GetRainfallReadings(stationId);

            // Assert
            response.Should().BeNull();
            _logger.VerifyLog(LogLevel.Error, Times.Exactly(1), "Json deserialization error.");
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task GetRainfallReadings_Should_ReturnNull_OnBadRequests(HttpStatusCode statusCode)
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode
                });

            // Act
            var response = await _weatherService.GetRainfallReadings(stationId);

            // Assert
            response.Should().BeNull();
            _logger.VerifyLog(LogLevel.Error, Times.Exactly(1), "HTTP request failed.");
        }
    }
}
