using System;
using System.Collections.Generic;
using System.Net;

namespace TeleSign.Services.Score
{
    /// <summary>
    /// Score provides risk information about a specified phone number.
    /// </summary>
    public class ScoreClient : TeleSignService
    {
        private const String SCORE_RESOURCE = "/v1/score/{0}";	
        public ScoreClient(TeleSignServiceConfiguration configuration) : base(configuration, null) { }

        /// <summary>
        /// Score is an API that delivers reputation scoring based on phone number intelligence, 
        /// traffic patterns, machine learning, and a global data consortium. 
        /// See https://developer.telesign.com/docs/rest_api-phoneid-score for detailed API documentation.         
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="accountLifecycleEvent"></param>        
        /// <param name="scoreParams"></param>
        /// <returns></returns>
        public TeleSignResponse Score(String phoneNumber, String accountLifecycleEvent, Dictionary<String, String> scoreParams = null) {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            if (null == scoreParams)
                scoreParams = new Dictionary<string, string>();
            scoreParams.Add("phone_number", phoneNumber);
            scoreParams.Add("account_lifecycle_event", accountLifecycleEvent);

            string resourceName = string.Format(SCORE_RESOURCE, phoneNumber);

            WebRequest request = this.ConstructWebRequest(resourceName, "POST", scoreParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }
    }
}
