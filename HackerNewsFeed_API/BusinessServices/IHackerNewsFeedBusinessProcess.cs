using System.Collections.Generic;
using System.Threading.Tasks;
using HackerNewsFeed_API.Shared;

namespace HackerNewsFeed_API.BusinessServices
{
    /// <summary>
    /// Defines the contract for fetching and processing Hacker News feed data.
    /// </summary>
    public interface IHackerNewsFeedBusinessProcess
    {
        /// <summary>
        /// Fetches the latest stories from Hacker News.
        /// </summary>
        /// <returns>The task result contains a ResponseMessage with a list of the latest stories.</returns>
        Task<ResponseMessage<List<StoryModal>>> GetLatestStoriesAsync();
    }
}