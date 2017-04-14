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
        public ScoreClient(TeleSignServiceConfiguration configuration = null) : base(configuration, null) { }
        /// <summary>
        /// Initializes a new instance of the ScoreClient class with a supplied credential and uri and
        /// a web requester. In general you do not need to use this constructor unless you want to intercept
        /// the web requests for logging/debugging/testing purposes.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        /// <param name="webRequester">The web requester to use.</param>
        public ScoreClient(
                    TeleSignServiceConfiguration configuration,
                    IWebRequester webRequester)
            : base(configuration, webRequester)
        {
        }

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
