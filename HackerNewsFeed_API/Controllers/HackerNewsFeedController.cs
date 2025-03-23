using System;
using Microsoft.AspNetCore.Mvc;
using HackerNewsFeed_API.BusinessServices;
using HackerNewsFeed_API.Shared;

namespace HackerNewsFeed_API.Controllers
{
    /// <summary>
    /// Controller for fetching top stories from the Hacker News API.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HackerNewsFeedController : ControllerBase
    {
        private readonly IHackerNewsFeedBusinessProcess _hackerNewsFeedBusinessProcess;
        private readonly ILogger<HackerNewsFeedController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HackerNewsFeedController"/> class.
        /// </summary>
        /// <param name="hackerNewsFeedBusinessProcess">The service to fetch stories from the Hacker News API.</param>
        /// <param name="logger">The logger for logging errors and information.</param>

        public HackerNewsFeedController(IHackerNewsFeedBusinessProcess hackerNewsFeedBusinessProcess,
                                             ILogger<HackerNewsFeedController> logger)
        {
            _hackerNewsFeedBusinessProcess = hackerNewsFeedBusinessProcess;
            _logger = logger;
        }

        /// <summary>
        /// Fetches the top stories from the Hacker News API.
        /// </summary>
        /// <returns>A list of top stories.</returns>

        [HttpGet("top")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseMessage<List<StoryModal>>>> GetTopStories()
        {
            _logger.LogInformation("Fetching top stories from the Hacker News API.");

            var response = await _hackerNewsFeedBusinessProcess.GetLatestStoriesAsync();

            if (!response.IsSuccess)
            {
                _logger.LogError("Failed to fetch top stories. Status code: {StatusCode}", response.StatusCode);
            }
            else
            {
                _logger.LogInformation("Successfully fetched {StoryCount} top stories.", response.Data?.Count ?? 0);
            }

            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}

