using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HackerNewsFeed_API.BusinessServices;
using HackerNewsFeed_API.Shared;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

public class HackerNewsFeedBusinessProcessTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly Mock<ILogger<HackerNewsFeedBusinessProcess>> _loggerMock;
    private readonly HttpClient _httpClient;
    private readonly HackerNewsFeedBusinessProcess _service;

    private const string TopStoriesEndpoint = "https://hacker-news.firebaseio.com/v0/topstories.json";
    private const string StoryDetailsEndpoint = "https://hacker-news.firebaseio.com/v0/item/{0}.json";

    public HackerNewsFeedBusinessProcessTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

        _cacheMock = new Mock<IMemoryCache>();
        _loggerMock = new Mock<ILogger<HackerNewsFeedBusinessProcess>>();

        _service = new HackerNewsFeedBusinessProcess(_httpClient, _cacheMock.Object, _loggerMock.Object);
    }


    [Fact]
    public async Task GetLatestStoriesAsync_ShouldReturnCachedStories_IfCacheExists()
    {
        // Arrange
        var cachedStories = new List<StoryModal>
        {
            new() { Title = "Cached Story", Url = "https://example.com/cached" }
        };

        _cacheMock.SetupCache("LatestStories", cachedStories);

        // Act
        var result = await _service.GetLatestStoriesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal("Cached Story", result.Data[0].Title);
    }

    [Fact]
    public async Task GetLatestStoriesAsync_ShouldReturnEmptyList_WhenApiReturnsNoStories()
    {
        // Arrange
        _httpMessageHandlerMock.SetupRequest(TopStoriesEndpoint, new int[] { });

        object cacheValue;
        _cacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(false);

        // Act
        var result = await _service.GetLatestStoriesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data);
    }


}

// Mocking Extensions
public static class MoqExtensions
{
    public static void SetupRequest<T>(this Mock<HttpMessageHandler> mock, string url, T response)
    {
        mock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == url),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(response))
            });
    }

    public static void SetupRequestFailure(this Mock<HttpMessageHandler> mock, string url)
    {
        mock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == url),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Simulated API failure"));
    }


    public static void SetupCache<T>(this Mock<IMemoryCache> mock, string key, T value)
    {
        object cacheValue = value;
        mock.Setup(m => m.TryGetValue(key, out cacheValue)).Returns(true);
    }
}
