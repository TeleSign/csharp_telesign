using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Telesign
{
    /// <summary>
    /// A set of APIs that deliver deep phone number data attributes that help optimize the end user
    /// verification process and evaluate risk.
    /// </summary>
    public class PhoneIdClient : RestClient
    {
        private const string SCORE_RESOURCE = "/v1/phoneid/{0}";

        public PhoneIdClient(string customerId,
                                string apiKey)
            : base(customerId,
                   apiKey)
        { }

        public PhoneIdClient(string customerId,
                                string apiKey,
                                string restEndPoint)
            : base(customerId,
                   apiKey,
                   restEndPoint)
        { }

        public PhoneIdClient(string customerId,
                                string apiKey,
                                string restEndPoint,
                                int timeout,
                                WebProxy proxy,
                                string proxyUsername,
                                string proxyPassword)
            : base(customerId,
                   apiKey,
                   restEndPoint,
                   timeout,
                   proxy,
                   proxyUsername,
                   proxyPassword)
        { }

        /// <summary>
        /// Generic TeleSign REST API request handler.
        /// </summary>
        /// <param name="resource">The partial resource URI to perform the request against.</param>
        /// <param name="method">The HTTP method name, as an upper case string.</param>
        /// <param name="parameters">Params to perform the request with. With application/json content type, Dictionary with string keys and object values should be used, otherwise string keys and string values</param>
        /// <returns></returns>
        protected TelesignResponse Execute(string resource, HttpMethod method, Dictionary<string, object> parameters)
        {
            return ExecuteAsync(resource, method, parameters).Result;
        }

        protected async Task<TelesignResponse> ExecuteAsync(string resource, HttpMethod method, Dictionary<string, object> parameters)
        {
            string contentType = "application/json";
            HttpRequestMessage request;
            string fieldsToSign = null;
            if (parameters == null)
                parameters = new Dictionary<string, object>();

            string resourceUri = string.Format("{0}{1}", this.restEndpoint, resource);

            fieldsToSign = JsonConvert.SerializeObject(parameters);
            request = new HttpRequestMessage(method, resourceUri);
            request.Content = new StringContent(fieldsToSign);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            Dictionary<string, string> headers = RestClient.GenerateTelesignHeaders(
                this.customerId,
                this.apiKey,
                method.ToString().ToUpper(),
                resource,
                fieldsToSign,
                null,
                null,
                RestClient.UserAgent,
                contentType);

            foreach (KeyValuePair<string, string> header in headers)
            {
                // .NET considers content-type header to be part of the content, not the request
                // when content-type is set as request header, exception is raised
                // proper way to set content-type is through content property of the request object
                if (header.Key == "Content-Type")
                    continue;
                request.Headers.Add(header.Key, header.Value);
            }

            HttpResponseMessage response = await this.httpClient.SendAsync(request).ConfigureAwait(false);

            TelesignResponse tsResponse = new TelesignResponse(response, isAsync: true);
            await tsResponse.Initialize();
            return tsResponse;
        }

        /// <summary>
        /// Generic TeleSign REST API POST handler.
        /// </summary>
        /// <param name="resource">The partial resource URI to perform the request against.</param>
        /// <param name="parameters">Body params to perform the POST request with.</param>
        /// <returns>The TelesignResponse for the request.</returns>
        public TelesignResponse Post(string resource, Dictionary<string, object> parameters)
        {
            return Execute(resource, HttpMethod.Post, parameters);
        }

        public Task<TelesignResponse> PostAsync(string resource, Dictionary<string, object> parameters)
        {
            return ExecuteAsync(resource, HttpMethod.Post, parameters);
        }

        /// <summary>
        /// The PhoneID API provides a cleansed phone number, phone type, and telecom carrier information to determine the
        /// best communication method - SMS or voice.
        /// 
        /// See https://developer.telesign.com/docs/phoneid-api for detailed API documentation.      
        /// </summary>
        public TelesignResponse PhoneId(string phoneNumber, Dictionary<string, object> phoneIdParams = null)
        {
            if (null == phoneIdParams)
                phoneIdParams = new Dictionary<string, object>();

            string resource = string.Format(SCORE_RESOURCE, phoneNumber);

            return Post(resource, phoneIdParams);
        }
        
        /// <summary>
        /// The PhoneID API provides a cleansed phone number, phone type, and telecom carrier information to determine the
        /// best communication method - SMS or voice.
        /// 
        /// See https://developer.telesign.com/docs/phoneid-api for detailed API documentation.      
        /// </summary>
        public Task<TelesignResponse> PhoneIdAsync(string phoneNumber, Dictionary<string, object> phoneIdParams = null)
        {
            if (null == phoneIdParams)
                phoneIdParams = new Dictionary<string, object>();

            string resource = string.Format(SCORE_RESOURCE, phoneNumber);

            return PostAsync(resource, phoneIdParams);
        }
    }
}
