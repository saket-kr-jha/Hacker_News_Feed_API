using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using HackerNewsFeed_API.BusinessServices;
using HackerNewsFeed_API.Controllers;
using HackerNewsFeed_API.Shared;

namespace HackerNewsFeed_API.Tests
{
    public class HackerNewsFeedControllerTests
    {
        private readonly Mock<IHackerNewsFeedBusinessProcess> _mockBusinessProcess;
        private readonly Mock<ILogger<HackerNewsFeedController>> _mockLogger;
        private readonly HackerNewsFeedController _controller;

        public HackerNewsFeedControllerTests()
        {
            _mockBusinessProcess = new Mock<IHackerNewsFeedBusinessProcess>();
            _mockLogger = new Mock<ILogger<HackerNewsFeedController>>();
            _controller = new HackerNewsFeedController(_mockBusinessProcess.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetTopStories_ShouldReturnOk_WhenServiceReturnsStories()
        {
            // Arrange
            var mockStories = new List<StoryModal>
            {
                new StoryModal { Title = "Story 1", Url = "http://story1.com" },
                new StoryModal { Title = "Story 2", Url = "http://story2.com" }
            };

            var successResponse = new ResponseMessage<List<StoryModal>>
            {
                Data = mockStories,
                IsSuccess = true,
                StatusCode = 200,
                Message = "Success"
            };

            _mockBusinessProcess.Setup(bp => bp.GetLatestStoriesAsync()).ReturnsAsync(successResponse);

            // Act
            var result = await _controller.GetTopStories();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, objectResult.StatusCode);
            var response = Assert.IsType<ResponseMessage<List<StoryModal>>>(objectResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(2, response.Data.Count);
        }

        [Fact]
        public async Task GetTopStories_ShouldReturn500_WhenServiceFails()
        {
            // Arrange
            var errorResponse = new ResponseMessage<List<StoryModal>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = 500,
                Message = "Internal Server Error"
            };

            _mockBusinessProcess.Setup(bp => bp.GetLatestStoriesAsync()).ReturnsAsync(errorResponse);

            // Act
            var result = await _controller.GetTopStories();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult.StatusCode);
            var response = Assert.IsType<ResponseMessage<List<StoryModal>>>(objectResult.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal("Internal Server Error", response.Message);
        }
    }
}
