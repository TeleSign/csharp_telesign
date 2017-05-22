using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Telesign
{
    /// <summary>
    /// The TeleSign RestClient is a generic HTTP REST client that can be extended to make requests against any of
    /// TeleSign's REST API endpoints.
    /// See https://developer.telesign.com for detailed API documentation.
    /// </summary>
    public class RestClient
    {
        public static readonly string UserAgent = string.Format("TeleSignSdk/csharp-{0} .Net/{1} HttpClient", 
            typeof(RestClient).Assembly.GetName().Version, 
            Environment.Version.ToString());

        private string customerId;
        private string apiKey;
        private string restEndpoint;
        private HttpClient httpClient;

        public RestClient(string customerId,
                          string apiKey,
                          string restEndpoint = "https://rest-api.telesign.com",
                          int timeout = 10,
                          WebProxy proxy = null,
                          string proxyUsername = null,
                          string proxyPassword = null)
        {
            this.customerId = customerId;
            this.apiKey = apiKey;
            this.restEndpoint = restEndpoint;

            if (proxy == null)
            {
                this.httpClient = new HttpClient();
            }
            else
            { 
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                httpClientHandler.Proxy = proxy;

                if (proxyUsername != null && proxyPassword != null)
                {
                    httpClientHandler.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                }

                this.httpClient = new HttpClient(httpClientHandler);
            }

            this.httpClient.Timeout = TimeSpan.FromSeconds(timeout);
        }

        /// <summary>
        /// A simple HTTP Response object.
        /// </summary>
        public class TelesignResponse
        {
            /// <summary>
            /// TeleSignResponse Constructor for initializing response variables.
            /// </summary>
            public TelesignResponse(HttpResponseMessage response)
            {
                this.StatusCode = (int)response.StatusCode;
                this.Headers = response.Headers;
                this.Body = response.Content.ReadAsStringAsync().Result;
                this.OK = response.IsSuccessStatusCode;

                try
                {
                    this.Json = JObject.Parse(this.Body);
                }
                catch (JsonReaderException ex)
                {
                    this.Json = new JObject();
                }
            }
            /// <summary>
            /// HttpStatus Code returned as part of response
            /// </summary>
            public int StatusCode { get; set; }
            /// <summary>
            /// Headers returned as part of response
            /// </summary>
            public HttpResponseHeaders Headers { get; set; }
            /// <summary>
            /// Response body in String
            /// </summary>
            public string Body { get; set; }
            /// <summary>
            /// Response as a Json Object
            /// </summary>
            public JObject Json { get; set; }
            /// <summary>
            /// Indicates if request was successfull
            /// </summary>
            public bool OK { get; set; }
        }

        /// <summary>
        /// Custom Authenticator for TeleSign REST APIs using RESTSharp
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        public static Dictionary<string, string> GenerateTelesignHeaders(string customerId,
                                                                         string apiKey,
                                                                         string methodName,
                                                                         string resource,
                                                                         string urlEncodedFields,
                                                                         string dateRfc2616,
                                                                         string nonce,
                                                                         string userAgent)
        {
            if (dateRfc2616 == null)
            {
                dateRfc2616 = DateTime.UtcNow.ToString("r");
            }

            if (nonce == null)
            {
                nonce = Guid.NewGuid().ToString();
            }

            string contentType = "";
            if (methodName == "POST" || methodName == "PUT")
            {
                contentType = "application/x-www-form-urlencoded";
            }

            string authMethod = "HMAC-SHA256";

            StringBuilder stringToSignBuilder = new StringBuilder();

            stringToSignBuilder.Append(string.Format("{0}", methodName));

            stringToSignBuilder.Append(string.Format("\n{0}", contentType));

            stringToSignBuilder.Append(string.Format("\n{0}", dateRfc2616));

            stringToSignBuilder.Append(string.Format("\nx-ts-auth-method:{0}", authMethod));

            stringToSignBuilder.Append(string.Format("\nx-ts-nonce:{0}", nonce));

            if (!string.IsNullOrEmpty(contentType) && !string.IsNullOrEmpty(urlEncodedFields))
            {
                stringToSignBuilder.Append(string.Format("\n{0}", urlEncodedFields));
            }

            stringToSignBuilder.Append(string.Format("\n{0}", resource));

            string stringToSign = stringToSignBuilder.ToString();

            HMAC hasher = new HMACSHA256(Convert.FromBase64String(apiKey));
            string signature = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

            string authorization = string.Format("TSA {0}:{1}", customerId, signature);

            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add("Authorization", authorization);
            headers.Add("Date", dateRfc2616);
            headers.Add("Content-Type", contentType);
            headers.Add("x-ts-auth-method", authMethod);
            headers.Add("x-ts-nonce", nonce);

            if (userAgent != null)
            {
                headers.Add("User-Agent", userAgent);
            }

            return headers;
        }

        /// <summary>
        /// Generic TeleSign REST API POST handler.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="postParams"></param>
        /// <returns></returns>
        public TelesignResponse Post(string resource, Dictionary<string, string> postParams)
        {
            return Execute(resource, HttpMethod.Post, postParams);
        }

        /// <summary>
        /// Generic TeleSign REST API GET handler.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="getParams"></param>
        /// <returns></returns>
        public TelesignResponse Get(string resource, Dictionary<string, string> getParams)
        {
            return Execute(resource, HttpMethod.Get, getParams);
        }

        /// <summary>
        /// Generic TeleSign REST API PUT handler.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="putParams"></param>
        /// <returns></returns>
        public TelesignResponse Put(string resource, Dictionary<string, string> putParams)
        {
            return Execute(resource, HttpMethod.Put, putParams);
        }

        /// <summary>
        /// Generic TeleSign REST API DELETE handler.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="deleteParams"></param>
        /// <returns></returns>
        public TelesignResponse Delete(string resource, Dictionary<string, string> deleteParams)
        {
            return Execute(resource, HttpMethod.Delete, deleteParams);
        }

        /// <summary>
        /// Generic API for executing requests.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>TeleSignResponse object</returns>
        private TelesignResponse Execute(string resource, HttpMethod method, Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }

            string resourceUri = string.Format("{0}{1}", this.restEndpoint, resource);

            FormUrlEncodedContent formBody = new FormUrlEncodedContent(parameters);
            string urlEncodedFields = formBody.ReadAsStringAsync().Result;

            HttpRequestMessage request;
            if (method == HttpMethod.Post || method == HttpMethod.Put)
            {
                request = new HttpRequestMessage(method, resourceUri);
                request.Content = formBody;
            }
            else
            {
                UriBuilder resourceUriWithQuery = new UriBuilder(resourceUri);
                resourceUriWithQuery.Query = urlEncodedFields;
                request = new HttpRequestMessage(method, resourceUriWithQuery.ToString());
            }

            Dictionary<string, string> headers = RestClient.GenerateTelesignHeaders(this.customerId,
                                                                                    this.apiKey,
                                                                                    method.ToString().ToUpper(),
                                                                                    resource,
                                                                                    urlEncodedFields,
                                                                                    null,
                                                                                    null,
                                                                                    RestClient.UserAgent);

            foreach (KeyValuePair<string, string> header in headers)
            {
                if (header.Key == "Content-Type")
                    // skip Content-Type for HttpClient
                    continue;

                request.Headers.Add(header.Key, header.Value);
            }

            HttpResponseMessage response = this.httpClient.SendAsync(request).Result;

            TelesignResponse tsResponse = new TelesignResponse(response);
            return tsResponse;
        }
    }
}
