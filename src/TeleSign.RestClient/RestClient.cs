using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Telesign.Sdk
{
    /// <summary>
    /// The TeleSign RestClient is a generic HTTP REST client that can be extended to make requests against any of
    /// TeleSign's REST API endpoints.
    /// See https://developer.telesign.com for detailed API documentation.
    /// </summary>
    public class RestClient
    {
        private readonly string userAgent = string.Format("TeleSignSdk/csharp-{0} .Net/{1} RestSharp", 
            typeof(RestClient).Assembly.GetName().Version, 
            Environment.Version.ToString());

        private string customerId { get; }
        private string apiKey { get; }
        private string restEndpoint { get; }

        private HttpClient httpClient;
        

        public RestClient(string customerId, 
                          string apiKey, 
                          string restEndpoint = null, 
                          int? timeout = null, 
                          int? readWriteTimeout = null, 
                          WebProxy proxy = null, 
                          string httpProxyUsername = null, 
                          string httpProxyPassword = null)
        {
            this.customerId = customerId;
            this.apiKey = apiKey;

            if (restEndpoint == null)
                this.restEndpoint = "https://rest-api.telesign.com";
            else
                this.restEndpoint = restEndpoint;

            if (timeout == null)                
                timeout = 10000;

            if (readWriteTimeout == null)
                readWriteTimeout = 10000;

            //client.UserAgent = string.Format("TeleSignSdk/CSharp-v{0} .Net{1}", typeof(RestClient).Assembly.GetName().Version, Environment.Version.ToString());
            //if (null != proxy)
            //{
            //    if (null != httpProxyUsername && null != httpProxyPassword)
            //    {
            //        proxy.Credentials = new NetworkCredential(httpProxyUsername, httpProxyPassword);
            //    }
            //    client.Proxy = proxy;
            //}

            //this.client = new RestSharp.RestClient(restEndpoint);
            this.httpClient = new HttpClient();
        }

        /// <summary>
        /// Generic API for executing requests.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>TeleSignResponse object</returns>
        private TeleSignResponse Execute(string resource, HttpMethod method, Dictionary<string, string> parameters)
        {
            if (parameters == null) {
                parameters = new Dictionary<string, string>();
            }

            string resourceUri = string.Format("{0}{1}", this.restEndpoint, resource);

            FormUrlEncodedContent formBody = new FormUrlEncodedContent(parameters);
            string urlEncodedFields = formBody.ReadAsStringAsync().Result;

            Dictionary<string, string> headers = RestClient.GenerateTelesignHeaders(this.customerId,
                                                                                    this.apiKey,
                                                                                    method.ToString().ToUpper(),
                                                                                    resource,
                                                                                    urlEncodedFields,
                                                                                    null,
                                                                                    null,
                                                                                    this.userAgent);
            
            HttpRequestMessage request = new HttpRequestMessage(method, resourceUri);
            request.Content = formBody;

            foreach (KeyValuePair<string, string> header in headers) {
                if (header.Key == "Content-Type")
                    // skip Content-Type due to nanny .net
                    continue;

                request.Headers.Add(header.Key, header.Value);
            }

            HttpResponseMessage response = this.httpClient.SendAsync(request).Result;

            TeleSignResponse tsResponse = new TeleSignResponse(response);
            return tsResponse;
        }

        /// <summary>
        /// Generic TeleSign REST API DELETE handler.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="deleteParams"></param>
        /// <returns></returns>
        public TeleSignResponse Delete(string resource, Dictionary<string, string> deleteParams)
        {
            return Execute(resource, HttpMethod.Delete, deleteParams);
        }
        /// <summary>
        /// Generic TeleSign REST API PUT handler.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="putParams"></param>
        /// <returns></returns>
        public TeleSignResponse Put(string resource, Dictionary<string, string> putParams)
        {
            return Execute(resource, HttpMethod.Put, putParams);
        }
        /// <summary>
        /// Generic TeleSign REST API POST handler.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="postParams"></param>
        /// <returns></returns>
        public TeleSignResponse Post(string resource, Dictionary<string, string> postParams)
        {
            return Execute(resource, HttpMethod.Post, postParams);
        }
        /// <summary>
        /// Generic TeleSign REST API GET handler.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="getParams"></param>
        /// <returns></returns>
        public TeleSignResponse Get(string resource, Dictionary<string, string> getParams)
        {
            return Execute(resource, HttpMethod.Get, getParams);
        }

        /// <summary>
        /// A simple HTTP Response object.
        /// </summary>
        public class TeleSignResponse
        {
            /// <summary>
            /// TeleSignResponse Constructor for initializing response variables.
            /// </summary>
            public TeleSignResponse(HttpResponseMessage response)
            {
                this.StatusCode = (int)response.StatusCode;
                this.Headers = response.Headers;
                this.Body = response.Content.ReadAsStringAsync().Result;
                this.OK = response.IsSuccessStatusCode;

                try {
                    this.Json = JObject.Parse(this.Body);
                } catch (JsonReaderException ex) {
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
                                                                         string userAgent) {
            if (dateRfc2616 == null) {
                dateRfc2616 = DateTime.UtcNow.ToString("r");
            }

            if (nonce == null) {
                nonce = Guid.NewGuid().ToString();
            }

            string contentType = "";
            if (methodName == "POST" || methodName == "PUT") {
                contentType = "application/x-www-form-urlencoded";
            }

            string authMethod = "HMAC-SHA256";

            StringBuilder stringToSignBuilder = new StringBuilder();

            stringToSignBuilder.Append(string.Format("{0}", methodName));

            stringToSignBuilder.Append(string.Format("\n{0}", contentType));

            stringToSignBuilder.Append(string.Format("\n{0}", dateRfc2616));

            stringToSignBuilder.Append(string.Format("\nx-ts-auth-method:{0}", authMethod));

            stringToSignBuilder.Append(string.Format("\nx-ts-nonce:{0}", nonce));

            if (!string.IsNullOrEmpty(contentType) && !string.IsNullOrEmpty(urlEncodedFields)) {
                stringToSignBuilder.Append(string.Format("\n{0}", urlEncodedFields));
            }

            stringToSignBuilder.Append(string.Format("\n{0}", resource));

            string stringToSign = stringToSignBuilder.ToString();

            string signature;

            byte[] bytes = Convert.FromBase64String(apiKey);

            HMAC hasher = new HMACSHA256(Convert.FromBase64String(apiKey));
            signature = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

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
        /// Encodes request parameters.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //public string GetEncodedBody(IRestRequest request)
        //{
        //    StringBuilder builder = new StringBuilder();

        //    foreach (Parameter p in request.Parameters.Where(p => p.Type.Equals(RestSharp.ParameterType.GetOrPost)))
        //    {
        //        builder.AppendFormat(
        //                            CultureInfo.InvariantCulture,
        //                            "{0}={1}&",
        //                            HttpUtility.UrlEncode(p.Name, Encoding.UTF8),
        //                            HttpUtility.UrlEncode(p.Value.ToString(), Encoding.UTF8));
        //    }
        //    if (builder.Length > 1)
        //    {
        //        builder.Length -= 1;
        //        return builder.ToString();
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}
    }
}
