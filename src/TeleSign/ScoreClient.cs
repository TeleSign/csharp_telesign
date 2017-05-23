using System.Collections.Generic;
using System.Net;

namespace Telesign
{
    /// <summary>
    /// Score provides risk information about a specified phone number.
    /// </summary>
    public class ScoreClient : RestClient
    {
        private const string SCORE_RESOURCE = "/v1/score/{0}";

        public ScoreClient(string customerId,
                                string apiKey)
            : base(customerId,
                   apiKey)
        { }

        public ScoreClient(string customerId,
                                string apiKey,
                                string restEndPoint)
            : base(customerId,
                   apiKey,
                   restEndPoint)
        { }

        public ScoreClient(string customerId,
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
        /// Score is an API that delivers reputation scoring based on phone number intelligence, traffic patterns, machine
        /// learning, and a global data consortium.
        /// 
        /// See https://developer.telesign.com/docs/score-api for detailed API documentation.     
        /// </summary>
        public TelesignResponse Score(string phoneNumber, string accountLifecycleEvent, Dictionary<string, string> scoreParams = null)
        {
            if (null == scoreParams)
                scoreParams = new Dictionary<string, string>();
            scoreParams.Add("phone_number", phoneNumber);
            scoreParams.Add("account_lifecycle_event", accountLifecycleEvent);

            string resource = string.Format(SCORE_RESOURCE, phoneNumber);

            return Post(resource, scoreParams);
        }
    }
}
