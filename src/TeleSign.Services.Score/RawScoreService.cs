using System;
using System.Collections.Generic;
using System.Net;

namespace TeleSign.Services.Score
{
    public class RawScoreService: TeleSignService
    {
        private const String SCORE_RESOURCE = "/v1/score/{0}";	
        public RawScoreService(TeleSignServiceConfiguration configuration) : base(configuration, null) { }

        public TeleSignResponse ScoreRaw(String phoneNumber, String accountLifecycleEvent, Dictionary<String, String> scoreParams = null) {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            if (null == scoreParams)
                scoreParams = new Dictionary<string, string>();
            scoreParams.Add("phone_number", phoneNumber);
            scoreParams.Add("account_lifecycle_event", accountLifecycleEvent);

            string resourceName = string.Format(SCORE_RESOURCE, phoneNumber);

            WebRequest request = this.ConstructWebRequest(resourceName, "POST", scoreParams, AuthenticationMethod.HmacSha256);

            return this.WebRequester.ReadTeleSignResponse(request);
        }
    }
}
