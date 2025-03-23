using System.Collections.Generic;
using Newtonsoft.Json;

namespace HackerNewsFeed_API.Shared
{
    /// <summary>
    /// Represents a story from the Hacker News API.
    /// </summary>
    public class StoryModal
    {
        /// <summary>
        /// The title of the story in HTML format.
        /// </summary>
        [JsonProperty("title")]
        public string? Title { get; set; }

        /// <summary>
        /// The URL of the story.
        /// </summary>
        [JsonProperty("url")]
        public string? Url { get; set; }

    }
}