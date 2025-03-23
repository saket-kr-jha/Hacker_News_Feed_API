using System;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using HackerNewsFeed_API.BusinessServices;
using HackerNewsFeed_API.Shared;

namespace HackerNewsFeed_API.BusinessServices
{
    /// <summary>
    /// Service to fetch latest stories from the Hacker News API.
    /// </summary>
    public class HackerNewsFeedBusinessProcess : IHackerNewsFeedBusinessProcess
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<HackerNewsFeedBusinessProcess> _logger;

        private const int MaxStories = 200;
        private const string CacheKey = "LatestStories";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        private const string BaseUrl = "https://hacker-news.firebaseio.com/v0";
        private const string TopStoriesEndpoint = $"{BaseUrl}/topstories.json";
        private const string StoryDetailsEndpoint = $"{BaseUrl}/item/{{0}}.json";

        /// <summary>
        /// Initializes a new instance of the <see cref="HackerNewsFeedBusinessProcess"/> class.
        /// </summary>
        public HackerNewsFeedBusinessProcess(HttpClient httpClient, IMemoryCache cache, ILogger<HackerNewsFeedBusinessProcess> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Fetches the latest stories from Hacker News.
        /// Uses caching to reduce redundant API calls.
        /// </summary>
        /// <returns>A list of the latest stories.</returns>
        public async Task<ResponseMessage<List<StoryModal>>> GetLatestStoriesAsync()
        {
            try
            {
                // Check cache first
                if (_cache.TryGetValue(CacheKey, out List<StoryModal>? cachedStories))
                {
                    _logger.LogInformation("Returning Hacker News Feed from Cache");
                    return SuccessResponse(cachedStories, "Stories retrieved from cache.");
                }

                _logger.LogInformation("Fetching latest stories from Hacker News API.");
                var storyIds = await FetchFromApiAsync<int[]>(TopStoriesEndpoint);

                if (storyIds == null || storyIds.Length == 0)
                {
                    _logger.LogWarning("Received empty story list from Hacker News.");
                    return SuccessResponse(new List<StoryModal>(), "No stories found.");
                }

                // Fetch stories concurrently
                var stories = await FetchStoriesAsync(storyIds.Take(MaxStories));

                // Store in cache
                _cache.Set(CacheKey, stories, CacheDuration);
                _logger.LogInformation("Stored latest stories in cache.");

                return SuccessResponse(stories, "Stories retrieved successfully.");
            }
            catch (Exception ex)
            {
                return HandleException<List<StoryModal>>(ex, "An error occurred while fetching latest stories.");
            }
        }

        /// <summary>
        /// Fetches a list of stories based on provided story IDs.
        /// </summary>
        private async Task<List<StoryModal>> FetchStoriesAsync(IEnumerable<int> storyIds)
        {
            var storyTasks = storyIds.Select(async id =>
            {
                var story = await FetchFromApiAsync<StoryModal>(string.Format(StoryDetailsEndpoint, id));
                if (story != null)
                {
                    return new StoryModal
                    {
                        Title = story?.Title,
                        Url = story?.Url
                    };
                }
                return null;
            });

            var stories = await Task.WhenAll(storyTasks);
            return stories.Where(story => story != null).ToList();
        }

        /// <summary>
        /// Generic method to fetch data from an API and handle exceptions.
        /// </summary>
        private async Task<T?> FetchFromApiAsync<T>(string url)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<T>(url);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed for URL: {Url}", url);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch data from URL: {Url}", url);
                return default;
            }
        }

        /// <summary>
        /// Generates a successful response message.
        /// </summary>
        private ResponseMessage<T> SuccessResponse<T>(T data, string message) =>
            new ResponseMessage<T>
            {
                Data = data,
                IsSuccess = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = message
            };

        /// <summary>
        /// Handles exceptions and returns a standardized error response.
        /// </summary>
        private ResponseMessage<T> HandleException<T>(Exception ex, string errorMessage)
        {
            _logger.LogError(ex, errorMessage);

            var statusCode = ex is HttpRequestException httpEx
                ? (int)(httpEx.StatusCode ?? HttpStatusCode.InternalServerError)
                : (int)HttpStatusCode.InternalServerError;

            return new ResponseMessage<T>
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = errorMessage,
                Description = ex.Message
            };
        }
    }
}
