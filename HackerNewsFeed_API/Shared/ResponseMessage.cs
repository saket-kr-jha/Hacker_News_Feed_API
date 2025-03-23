using System;
namespace HackerNewsFeed_API.Shared
{
    /// <summary>
    /// Response message
    /// </summary>
    /// <typeparam name="T">The type of the data contained in the response.</typeparam>
    public class ResponseMessage<T>
    {
        /// <summary>
        /// Response Object
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code of the response.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a short message describing the result of the operation.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets a detailed description of the result or error.
        /// </summary>
        public string? Description { get; set; }
    }
}

