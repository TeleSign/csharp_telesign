using System;
using System.Collections.Generic;
using System.Net;

namespace Telesign
{
    /// <summary>
    /// AutoVerify is a secure, lightweight SDK that integrates a frictionless user verification process into existing native mobile applications.
    /// </summary>
    public class AutoVerifyClient : RestClient
    {
        private const string AUTOVERIFY_STATUS_RESOURCE = "/v1/mobile/verification/status/{0}";

        public AutoVerifyClient(string customerId,
                                string apiKey)
            : base(customerId,
                   apiKey)
        { }

        public AutoVerifyClient(string customerId,
                                string apiKey,
                                string restEndPoint)
            : base(customerId,
                   apiKey,
                   restEndPoint)
        { }

        public AutoVerifyClient(string customerId,
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
        /// Retrieves the verification result for an AutoVerify transaction by external_id.To ensure a secure verification flow you must check the status using TeleSign's servers on your backend. Do not rely on the SDK alone to indicate a successful verification.See<a href="https://developer.telesign.com/docs/auto-verify-sdk#section-obtaining-verification-status"> for detailed API documentation</a>.
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="restParams"></param>
        /// <returns></returns>
        public TelesignResponse Status(string externalId, Dictionary<string, string> parameters = null)
        {
            return this.Get(string.Format(AUTOVERIFY_STATUS_RESOURCE, externalId), parameters);
        }
    }
}
