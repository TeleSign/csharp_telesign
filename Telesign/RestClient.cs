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
    /// 
    /// See https://developer.telesign.com for detailed API documentation.
    /// </summary>
    public class RestClient : IDisposable
    {
        public static readonly string UserAgent = string.Format("TeleSignSdk/csharp-{0} .Net/{1} HttpClient",
            "2.2.0",
            Environment.Version.ToString());

        private string customerId;
        private string apiKey;
        private string restEndpoint;
        private HttpClient httpClient;

        bool disposed = false;

        /// <summary>
        /// TeleSign RestClient useful for making generic RESTful requests against our API.
        /// </summary>
        /// <param name="customerId">Your customer_id string associated with your account.</param>
        /// <param name="apiKey">Your api_key string associated with your account.</param>
        /// <param name="restEndpoint">Override the default restEndpoint to target another endpoint.</param>
        /// <param name="timeout">The timeout passed into HttpClient.</param>
        /// <param name="proxy">The proxy passed into HttpClient.</param>
        /// <param name="proxyUsername">The username passed into HttpClient.</param>
        /// <param name="proxyPassword">The password passed into HttpClient.</param>
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            this.httpClient.Dispose();
            disposed = true;
        }

        /// <summary>
        /// A simple HTTP Response object to abstract the underlying HttpClient library response.
        /// </summary>
        public class TelesignResponse
        {
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
                catch (JsonReaderException)
                {
                    this.Json = new JObject();
                }
            }

            public int StatusCode { get; set; }
            public HttpResponseHeaders Headers { get; set; }
            public string Body { get; set; }
            public JObject Json { get; set; }
            public bool OK { get; set; }
        }

        /// <summary>
        /// Generates the TeleSign REST API headers used to authenticate requests.
        ///
        /// Creates the canonicalized stringToSign and generates the HMAC signature.This is used to authenticate requests
        /// against the TeleSign REST API.
        ///
        /// See https://developer.telesign.com/docs/authentication for detailed API documentation.
        /// </summary>
        /// <param name="customerId">Your account customer_id.</param>
        /// <param name="apiKey">Your account api_key.</param>
        /// <param name="methodName">The HTTP method name of the request as a upper case string, should be one of 'POST', 'GET', 'PUT' or 'DELETE'.</param>
        /// <param name="resource">The partial resource URI to perform the request against.</param>
        /// <param name="urlEncodedFields">URL encoded HTTP body to perform the HTTP request with.</param>
        /// <param name="dateRfc2616">The date and time of the request formatted in rfc 2616.</param>
        /// <param name="nonce">A unique cryptographic nonce for the request.</param>
        /// <param name="userAgent">User Agent associated with the request.</param>
        /// <returns>A dictionary of HTTP headers to be applied to the request.</returns>
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
        /// <param name="resource">The partial resource URI to perform the request against.</param>
        /// <param name="parameters">Body params to perform the POST request with.</param>
        /// <returns>The TelesignResponse for the request.</returns>
        public TelesignResponse Post(string resource, Dictionary<string, string> parameters)
        {
            return Execute(resource, HttpMethod.Post, parameters);
        }

        /// <summary>
        /// Generic TeleSign REST API GET handler.
        /// </summary>
        /// <param name="resource">The partial resource URI to perform the request against.</param>
        /// <param name="parameters">Body params to perform the GET request with.</param>
        /// <returns>The TelesignResponse for the request.</returns>
        public TelesignResponse Get(string resource, Dictionary<string, string> parameters)
        {
            return Execute(resource, HttpMethod.Get, parameters);
        }

        /// <summary>
        /// Generic TeleSign REST API PUT handler.
        /// </summary>
        /// <param name="resource">The partial resource URI to perform the request against.</param>
        /// <param name="parameters">Body params to perform the PUT request with.</param>
        /// <returns>The TelesignResponse for the request.</returns>
        public TelesignResponse Put(string resource, Dictionary<string, string> parameters)
        {
            return Execute(resource, HttpMethod.Put, parameters);
        }

        /// <summary>
        /// Generic TeleSign REST API DELETE handler.
        /// </summary>
        /// <param name="resource">The partial resource URI to perform the request against.</param>
        /// <param name="parameters">Body params to perform the DELETE request with.</param>
        /// <returns>The TelesignResponse for the request.</returns>
        public TelesignResponse Delete(string resource, Dictionary<string, string> parameters)
        {
            return Execute(resource, HttpMethod.Delete, parameters);
        }

        /// <summary>
        /// Generic TeleSign REST API request handler.
        /// </summary>
        /// <param name="resource">The partial resource URI to perform the request against.</param>
        /// <param name="method">The HTTP method name, as an upper case string.</param>
        /// <param name="parameters">Params to perform the request with.</param>
        /// <returns></returns>
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
                    // skip Content-Type, otherwise HttpClient will complain
                    continue;

                request.Headers.Add(header.Key, header.Value);
            }

            HttpResponseMessage response = this.httpClient.SendAsync(request).Result;

            TelesignResponse tsResponse = new TelesignResponse(response);
            return tsResponse;
        }
    }
}
