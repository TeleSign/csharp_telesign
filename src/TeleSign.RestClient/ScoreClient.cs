using System;
using System.Collections.Generic;
using System.Net;

namespace TeleSign.RestClient
{
    /// <summary>
    /// Score provides risk information about a specified phone number.
    /// </summary>
    public class ScoreClient : TeleSignRestClient
    {
        private const string SCORE_RESOURCE = "/v1/score/{0}";	
        public ScoreClient(string customerId, string apiKey, string restEndPoint, int timeout = 10000, int readWriteTimeout = 10000, WebProxy proxy = null, string httpProxyUsername = null, string httpProxyPassword = null) : base(customerId, apiKey, restEndPoint, timeout, readWriteTimeout, proxy, httpProxyUsername, httpProxyPassword) { }
        
        /// <summary>
        /// Score is an API that delivers reputation scoring based on phone number intelligence, 
        /// traffic patterns, machine learning, and a global data consortium. 
        /// See https://developer.telesign.com/docs/rest_api-phoneid-score for detailed API documentation.         
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="accountLifecycleEvent"></param>        
        /// <param name="scoreParams"></param>
        /// <returns></returns>
        public TeleSignResponse Score(string phoneNumber, string accountLifecycleEvent, Dictionary<string, string> scoreParams = null) {            

            if (null == scoreParams)
                scoreParams = new Dictionary<string, string>();
            scoreParams.Add("phone_number", phoneNumber);
            scoreParams.Add("account_lifecycle_event", accountLifecycleEvent);

            string resource = string.Format(SCORE_RESOURCE, phoneNumber);            

            return Post(resource, scoreParams);
        }
    }
}
