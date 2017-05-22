using System;
using System.Collections.Generic;
using System.Net;

namespace Telesign.Sdk
{
    /// <summary>
    /// Score provides risk information about a specified phone number.
    /// </summary>
    public class PhoneIdClient : RestClient
    {
        private const string SCORE_RESOURCE = "/v1/score/{0}";	
        public PhoneIdClient(string customerId, string apiKey, string restEndPoint, int timeout = 10000, int readWriteTimeout = 10000, WebProxy proxy = null, string httpProxyUsername = null, string httpProxyPassword = null) : base(customerId, apiKey, restEndPoint, timeout, readWriteTimeout, proxy, httpProxyUsername, httpProxyPassword) { }
        
        /// <summary>
        /// Score is an API that delivers reputation scoring based on phone number intelligence, 
        /// traffic patterns, machine learning, and a global data consortium. 
        /// See https://developer.telesign.com/docs/rest_api-phoneid-score for detailed API documentation.         
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="accountLifecycleEvent"></param>        
        /// <param name="scoreParams"></param>
        /// <returns></returns>
        public TeleSignResponse PhoneId(string phoneNumber, Dictionary<string, string> phoneIdParams = null) {            

            
            if (null == phoneIdParams)
                phoneIdParams = new Dictionary<string, string>();

            string resource = string.Format(SCORE_RESOURCE, phoneNumber);            

            return Post(resource, phoneIdParams);
        }
    }
}
