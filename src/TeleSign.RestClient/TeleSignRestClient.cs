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
    public class TeleSignRestClient
    {
        private string customerId { get; }
        private string apiKey { get; }

        RestSharp.RestClient client;

        public TeleSignRestClient(string customerId, string apiKey, string restEndpoint= null, WebProxy proxy = null, string httpProxyUsername = null, string httpProxyPassword = null) {
            this.customerId = customerId;
            this.apiKey = apiKey;
            
            if (!string.IsNullOrEmpty(restEndpoint))
                client = new RestSharp.RestClient(restEndpoint);
            else
                client = new RestSharp.RestClient("https://rest-api.telesign.com");

            client.UserAgent = string.Format("TeleSignSdk/CSharp-v{0} .Net{1}", typeof(TeleSignRestClient).Assembly.GetName().Version, Environment.Version.ToString());
            if (null != proxy) {
                if (null != httpProxyUsername && null != httpProxyPassword) {
                    proxy.Credentials = new NetworkCredential(httpProxyUsername, httpProxyPassword);
                }
                client.Proxy = proxy;
            }
        }

        public TeleSignResponse Execute(string resourceName, Method method, Dictionary<string, string> parameters) {          
            var request = new RestRequest(resourceName, method);
            string contentType = "application/x-www-form-urlencoded";
            foreach (KeyValuePair<string, string> param in parameters)
                request.AddParameter(param.Key, param.Value, contentType, ParameterType.GetOrPost);

            client.Authenticator = new TeleSignAuthenticator(customerId, apiKey);            
            
            IRestResponse response = client.Execute(request);
            TeleSignResponse tsResponse = new TeleSignResponse(response);
            return tsResponse;
        }
        
        public TeleSignResponse Delete(string resource, Dictionary<string, string> deleteParams)
        {
            return Execute(resource, Method.DELETE, deleteParams);
        }
        public TeleSignResponse Put(string resource, Dictionary<string, string> putParams)
        {
            return Execute(resource, Method.PUT, putParams);
        }
        public TeleSignResponse Post(string resource, Dictionary<string, string> postParams) {
            return Execute(resource, Method.POST, postParams);
        }
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
            /// TeleSignResponse Constructor initializes Headers member variable.
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
            public bool Ok { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class TeleSignAuthenticator : RestSharp.Authenticators.IAuthenticator
        {
            private string customerId { get; }
            private string secretKey { get; }

            public TeleSignAuthenticator(string customerId, string secretKey) {
                this.customerId = customerId;
                this.secretKey = secretKey;
            }
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
                else {
                    return string.Empty;
                }                
            }           
        }
    }
}
