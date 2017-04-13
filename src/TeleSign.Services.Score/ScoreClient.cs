using System;
using System.Collections.Generic;

namespace TeleSign.Services.Score
{
    /// <summary>
    /// Score provides risk information about a specified phone number.
    /// </summary>
    public class ScoreClient:RawScoreService
    {
        public ScoreClient(TeleSignServiceConfiguration configuration): base(configuration) { }

        /// <summary>
        /// Score is an API that delivers reputation scoring based on phone number intelligence, traffic patterns, machine learning, and a global data consortium. See https://developer.telesign.com/docs/rest_api-phoneid-score for detailed API documentation.         
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="accountLifecycleEvent"></param>        
        /// <param name="scoreParams"></param>
        /// <returns></returns>
        public TeleSignResponse Score(String phoneNumber, String accountLifecycleEvent, Dictionary<String, String> scoreParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);
            CheckArgument.NotNullOrEmpty(accountLifecycleEvent, "account_lifecycle_event");

            TeleSignResponse response = new TeleSignResponse();
            try
            {
                response = this.ScoreRaw(phoneNumber, accountLifecycleEvent, scoreParams);
            }
            catch (Exception e)
            {
                throw new ResponseParseException(
                            "Error parsing Score response",
                            response.ToString(),
                            e);
            }
            return response;
        }
    }
}
