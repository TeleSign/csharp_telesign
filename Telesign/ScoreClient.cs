using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Telesign
{
    /// <summary>
    /// ScoreClient for TeleSign Intelligence Cloud.
    /// Supports POST /intelligence/phone endpoint only (Cloud migration).
    /// Sends phone number and parameters in request body as form-urlencoded.
    /// See https://developer.telesign.com/enterprise/docs/intelligence-cloud-get-started for documentation.
    /// </summary>
    public class ScoreClient : RestClient
    {
        private const string DETECT_HOST = "https://detect.telesign.com";
        private const string INTELLIGENCE_RESOURCE = "/intelligence/phone";
        public ScoreClient(string customerId,
                                string apiKey)
            : base(customerId,
                   apiKey,
                   DETECT_HOST)
        { }

        public ScoreClient(string customerId,
                                string apiKey,
                                string restEndPoint,
                                string source,
                                string sdkVersionOrigin,
                                string sdkVersionDependency)
            : base(customerId,
                   apiKey,
                   restEndPoint,
                   source: source,
                   sdkVersionOrigin: sdkVersionOrigin,
                   sdkVersionDependency: sdkVersionDependency)
        { }

        public ScoreClient(string customerId,
                                string apiKey,
                                string restEndPoint,
                                int timeout,
                                WebProxy proxy,
                                string proxyUsername,
                                string proxyPassword,
                                string source,
                                string sdkVersionOrigin,
                                string sdkVersionDependency)
            : base(customerId,
                   apiKey,
                   restEndPoint,
                   timeout,
                   proxy,
                   proxyUsername,
                   proxyPassword,
                   source,
                   sdkVersionOrigin,
                   sdkVersionDependency)
        { }
        
        /// <summary>
        /// Obtain a risk recommendation for this phone number, as well as other relevant information using Telesign Intellegence Cloud API.
        /// Sends phone number and event in POST body encoded as application/x-www-form-urlencoded.
        /// Optional parameters include account_id, device_id, email_address, external_id, and originating_ip.
        /// API: POST https://detect.telesign.com/intelligence/phone
        /// See https://developer.telesign.com/enterprise/reference/submitphonenumberforintelligencecloud for detailed API documentation.   
        /// </summary>
        public TelesignResponse Score(string phoneNumber, string accountLifecycleEvent, Dictionary<string, string> scoreParams = null)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("phoneNumber cannot be null or empty");
            if (string.IsNullOrEmpty(accountLifecycleEvent))
                throw new ArgumentException("accountLifecycleEvent cannot be null or empty");

            if (scoreParams == null)
                scoreParams = new Dictionary<string, string>();

            scoreParams["phone_number"] = phoneNumber;
            scoreParams["account_lifecycle_event"] = accountLifecycleEvent;

            return Post(INTELLIGENCE_RESOURCE, scoreParams);
        }

        /// <summary>
        /// Obtain a risk recommendation for this phone number, as well as other relevant information using Telesign Intellegence Cloud API asynchronously. 
        /// Sends phone number and event in POST body encoded as application/x-www-form-urlencoded.
        /// Optional parameters include account_id, device_id, email_address, external_id, and originating_ip.
        /// API: POST https://detect.telesign.com/intelligence/phone
        /// See https://developer.telesign.com/enterprise/reference/submitphonenumberforintelligencecloud for detailed API documentation. 
        /// </summary>
        public Task<TelesignResponse> ScoreAsync(string phoneNumber, string accountLifecycleEvent, Dictionary<string, string> scoreParams = null)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("phoneNumber cannot be null or empty");
            if (string.IsNullOrEmpty(accountLifecycleEvent))
                throw new ArgumentException("accountLifecycleEvent cannot be null or empty");

            if (scoreParams == null)
                scoreParams = new Dictionary<string, string>();

            scoreParams["phone_number"] = phoneNumber;
            scoreParams["account_lifecycle_event"] = accountLifecycleEvent;

            return PostAsync(INTELLIGENCE_RESOURCE, scoreParams);
        }
    }

}