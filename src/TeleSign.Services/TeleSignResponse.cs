using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeleSign.Services
{
    /// <summary>
    /// A simple HTTP Response object.
    /// </summary>
    public class TeleSignResponse
    {
        /// <summary>
        /// TeleSignResponse Constructor initializes Headers member variable.
        /// </summary>
        public TeleSignResponse() {
            Headers = new Dictionary<string, string[]>();
        }
        /// <summary>
        /// HttpStatus Code returned as part of response
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// HttpStatus StatusDescription returned as part of response
        /// </summary>
        public string StatusLine { get; set; }
        /// <summary>
        /// Headers returned as part of response
        /// </summary>
        public Dictionary<String, String[]> Headers { get; set; }
        /// <summary>
        /// Response body in String
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Response as a Json Object
        /// </summary>
        public JObject Json { get; set; }
        /// <summary>
        /// utility method to add header returned as part of response
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="headerValue"></param>
        public void addHeader(string headerName, string[] headerValue) {
            Headers.Add(headerName, headerValue);
        }
    }
}
