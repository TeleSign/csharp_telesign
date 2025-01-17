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
