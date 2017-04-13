using System;
using System.Collections.Generic;
using System.Net;

namespace TeleSign.Services.AutoVerify
{
    /// <summary>
    /// AutoVerify is a secure, lightweight SDK that integrates a frictionless user verification process into existing native mobile applications.
    /// </summary>
    public class AutoVerifyClient: TeleSignService
    {
        private const string AUTOVERIFY_STATUS_RESOURCE = "/v1/mobile/verification/status/{0}";

        public AutoVerifyClient(TeleSignServiceConfiguration configuration) :base(configuration, null) { }
        /// <summary>
        /// Initializes a new instance of the AutoVerifyClient class with a supplied credential and uri and
        /// a web requester. In general you do not need to use this constructor unless you want to intercept
        /// the web requests for logging/debugging/testing purposes.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        /// <param name="webRequester">The web requester to use.</param>
        public AutoVerifyClient(
                    TeleSignServiceConfiguration configuration,
                    IWebRequester webRequester)
            : base(configuration, webRequester)
        {
        }

        /// <summary>
        /// Retrieves the verification result for an AutoVerify transaction by external_id.To ensure a secure verification flow you must check the status using TeleSign's servers on your backend. Do not rely on the SDK alone to indicate a successful verification.See<a href="https://developer.telesign.com/docs/auto-verify-sdk#section-obtaining-verification-status"> for detailed API documentation</a>.
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="statusParams"></param>
        /// <returns></returns>
        public TeleSignResponse Status(string externalId, Dictionary<String, String> statusParams = null) {
            if (null == statusParams)
                statusParams = new Dictionary<string, string>();

            statusParams.Add("external_id", externalId);

            string resourceName = string.Format(AUTOVERIFY_STATUS_RESOURCE, externalId);

            WebRequest request = this.ConstructWebRequest(resourceName, "GET", statusParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }
    }
}
