using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RestSharp;
using Newtonsoft.Json.Linq;
using RestSharp.Extensions.MonoHttp;
using System.Security.Cryptography;
using System.Net;
using Newtonsoft.Json;

namespace TeleSign.RestClient
{
    /// <summary>
    /// The TeleSign RestClient is a generic HTTP REST client that can be extended to make requests against any of
    /// TeleSign's REST API endpoints.
    /// See https://developer.telesign.com for detailed API documentation.
    /// </summary>
    public class TeleSignRestClient
    {
        private string customerId { get; }
        private string apiKey { get; }

        RestSharp.RestClient client;

        public TeleSignRestClient(string customerId, string apiKey, string restEndpoint = null, int timeout = 10000, int readWriteTimeout = 10000, WebProxy proxy = null, string httpProxyUsername = null, string httpProxyPassword = null)
        {
            this.customerId = customerId;
            this.apiKey = apiKey;

            if (!string.IsNullOrEmpty(restEndpoint))
                client = new RestSharp.RestClient(restEndpoint);
            else
                client = new RestSharp.RestClient("https://rest-api.telesign.com");

            if (!(timeout == 10000))                
                client.Timeout = timeout;

            if (!(readWriteTimeout == 10000))                
                client.ReadWriteTimeout = readWriteTimeout;

            client.UserAgent = string.Format("TeleSignSdk/CSharp-v{0} .Net{1}", typeof(TeleSignRestClient).Assembly.GetName().Version, Environment.Version.ToString());
            if (null != proxy)
            {
                if (null != httpProxyUsername && null != httpProxyPassword)
                {
                    proxy.Credentials = new NetworkCredential(httpProxyUsername, httpProxyPassword);
                }
                client.Proxy = proxy;
            }
        }

        /// <summary>
        /// Generic API for executing requests.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>TeleSignResponse object</returns>
        public TeleSignResponse Execute(string resourceName, Method method, Dictionary<string, string> parameters)
        {
            var request = new RestRequest(resourceName, method);
            string contentType = "application/x-www-form-urlencoded";
            foreach (KeyValuePair<string, string> param in parameters)
                request.AddParameter(param.Key, param.Value, contentType, ParameterType.GetOrPost);

            client.Authenticator = new TeleSignAuthenticator(customerId, apiKey);

            IRestResponse response = client.Execute(request);
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
            return Execute(resource, Method.DELETE, deleteParams);
        }
        /// <summary>
        /// Generic TeleSign REST API PUT handler.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="putParams"></param>
        /// <returns></returns>
        public TeleSignResponse Put(string resource, Dictionary<string, string> putParams)
        {
            return Execute(resource, Method.PUT, putParams);
        }
        /// <summary>
        /// Generic TeleSign REST API POST handler.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="postParams"></param>
        /// <returns></returns>
        public TeleSignResponse Post(string resource, Dictionary<string, string> postParams)
        {
            return Execute(resource, Method.POST, postParams);
        }
        /// <summary>
        /// Generic TeleSign REST API GET handler.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="getParams"></param>
        /// <returns></returns>
        public TeleSignResponse Get(string resource, Dictionary<string, string> getParams)
        {
            return Execute(resource, Method.GET, getParams);
        }

        /// <summary>
        /// A simple HTTP Response object.
        /// </summary>
        public class TeleSignResponse
        {
            /// <summary>
            /// TeleSignResponse Constructor for initializing response variables.
            /// </summary>
            public TeleSignResponse(IRestResponse response)
            {
                Headers = new Dictionary<string, string>();
                HttpStatusCode code = response.StatusCode;
                this.StatusCode = (int)code;
                Headers = response.Headers.ToDictionary(k => k.Name, v => v.Value.ToString());
                if (response.ResponseStatus == ResponseStatus.Completed && (200 <= StatusCode && StatusCode <= 299))
                {
                    Ok = true;
                    Body = response.Content;
                    try
                    {
                        Json = JObject.Parse(Body);
                    }
                    catch (JsonReaderException ex)
                    {
                        Json = new JObject();
                    }
                }
                else
                {
                    Ok = false;
                    Body = string.Empty;
                    Json = new JObject();
                    if (response.ErrorException != null)
                        ErrorMessage = response.ErrorMessage;

                }
            }
            /// <summary>
            /// HttpStatus Code returned as part of response
            /// </summary>
            public int StatusCode { get; set; }
            /// <summary>
            /// Headers returned as part of response
            /// </summary>
            public Dictionary<String, String> Headers { get; set; }
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
            public bool Ok { get; set; }
            /// <summary>
            /// Error message if returned by response
            /// </summary>
            public string ErrorMessage { get; set; }
        }

        public class TeleSignAuthenticator : RestSharp.Authenticators.IAuthenticator
        {
            private string customerId { get; }
            private string secretKey { get; }

            /// <summary>
            /// The TeleSignAuthenticator constructor
            /// </summary>
            /// <param name="customerId"></param>
            /// <param name="secretKey"></param>
            public TeleSignAuthenticator(string customerId, string secretKey)
            {
                this.customerId = customerId;
                this.secretKey = secretKey;
            }
            /// <summary>
            /// Custom Authenticator for TeleSign REST APIs using RESTSharp
            /// </summary>
            /// <param name="client"></param>
            /// <param name="request"></param>
            public void Authenticate(IRestClient client, IRestRequest request)
            {
                string resourceName = request.Resource;
                DateTime timeStamp = DateTime.UtcNow;
                string dateRfc2616 = timeStamp.ToString("r");
                string nonce = Guid.NewGuid().ToString();
                string contentType = string.Empty;
                string encodedBody = string.Empty;
                string authMethod = "HMAC-SHA256";
                if (request.Method == RestSharp.Method.PUT || request.Method == RestSharp.Method.POST)
                {
                    contentType = "application/x-www-form-urlencoded";
                    encodedBody = GetEncodedBody(request);
                    byte[] body = Encoding.UTF8.GetBytes(encodedBody);
                    request.AddParameter(contentType, body, ParameterType.RequestBody);
                }

                StringBuilder stringToSignBuilder = new StringBuilder();
                stringToSignBuilder.Append(string.Format("{0}", request.Method.ToString()));

                stringToSignBuilder.Append(string.Format("\n{0}", contentType));

                stringToSignBuilder.Append(string.Format("\n{0}", dateRfc2616));

                stringToSignBuilder.Append(string.Format("\nx-ts-auth-method:{0}", authMethod));

                stringToSignBuilder.Append(string.Format("\nx-ts-nonce:{0}", nonce));

                if (!string.IsNullOrEmpty(contentType) && !string.IsNullOrEmpty(encodedBody))
                {
                    stringToSignBuilder.Append(string.Format("\n{0}", encodedBody));
                }

                stringToSignBuilder.Append(string.Format("\n{0}", resourceName));

                string stringToSign = stringToSignBuilder.ToString();

                byte[] secretKeyBytes = Convert.FromBase64String(secretKey);
                byte[] stringToSignBytes = Encoding.UTF8.GetBytes(stringToSign);

                HMAC hasher = new HMACSHA256(secretKeyBytes);

                string signature = Convert.ToBase64String(hasher.ComputeHash(stringToSignBytes));


                string authorization = string.Format("TSA {0}:{1}", customerId, signature);

                request.AddHeader("Authorization", authorization);
                request.AddHeader("Date", dateRfc2616);
                request.AddHeader("Content-Type", contentType);
                request.AddHeader("x-ts-auth-method", authMethod);
                request.AddHeader("x-ts-nonce", nonce);

            }

            /// <summary>
            /// Encodes request parameters.
            /// </summary>
            /// <param name="request"></param>
            /// <returns></returns>
            public string GetEncodedBody(IRestRequest request)
            {
                StringBuilder builder = new StringBuilder();

                foreach (Parameter p in request.Parameters.Where(p => p.Type.Equals(RestSharp.ParameterType.GetOrPost)))
                {
                    builder.AppendFormat(
                                        CultureInfo.InvariantCulture,
                                        "{0}={1}&",
                                        HttpUtility.UrlEncode(p.Name, Encoding.UTF8),
                                        HttpUtility.UrlEncode(p.Value.ToString(), Encoding.UTF8));
                }
                if (builder.Length > 1)
                {
                    builder.Length -= 1;
                    return builder.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
