using System;
using System.Collections.Generic;
using System.Net;

namespace Telesign
{
    /// <summary>
    /// Score provides risk information about a specified phone number.
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
        /// Score is an API that delivers reputation scoring based on phone number intelligence, 
        /// traffic patterns, machine learning, and a global data consortium. 
        /// See https://developer.telesign.com/docs/rest_api-phoneid-score for detailed API documentation.         
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="accountLifecycleEvent"></param>        
        /// <param name="scoreParams"></param>
        /// <returns></returns>
        public TelesignResponse PhoneId(string phoneNumber, Dictionary<string, string> phoneIdParams = null) {            

            
            if (null == phoneIdParams)
                phoneIdParams = new Dictionary<string, string>();

            string resource = string.Format(SCORE_RESOURCE, phoneNumber);            

            return Post(resource, phoneIdParams);
        }
    }
}
