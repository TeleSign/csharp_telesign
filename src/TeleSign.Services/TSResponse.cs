using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeleSign.Services
{
    public class TSResponse
    {
        public TSResponse() {
            Headers = new Dictionary<string, string[]>();
        }
        public int StatusCode { get; set; }
        public Dictionary<String, String[]> Headers { get; set; }
        public string BodyinString { get; set; }
        public JObject JsonBody { get; set; }
        public void addHeader(string headerName, string[] headerValue) {
            Headers.Add(headerName, headerValue);
        }
    }
}
