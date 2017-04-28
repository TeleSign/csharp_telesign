using System;
using System.Collections.Generic;
using System.Net;

namespace TeleSign.RestClient
{
    /// <summary>
    /// AutoVerify is a secure, lightweight SDK that integrates a frictionless user verification process into existing native mobile applications.
    /// </summary>
    public class AutoVerifyClient: TeleSignRestClient
    {
        private const string AUTOVERIFY_STATUS_RESOURCE = "/v1/mobile/verification/status/{0}";

        public AutoVerifyClient(string customerId, string apiKey, string restEndPoint, int timeout = 10000, int readWriteTimeout = 10000, WebProxy proxy = null, string httpProxyUsername = null, string httpProxyPassword = null) : base(customerId, apiKey, restEndPoint, timeout, readWriteTimeout, proxy, httpProxyUsername, httpProxyPassword) { }
        
        /// <summary>
        /// Retrieves the verification result for an AutoVerify transaction by external_id.To ensure a secure verification flow you must check the status using TeleSign's servers on your backend. Do not rely on the SDK alone to indicate a successful verification.See<a href="https://developer.telesign.com/docs/auto-verify-sdk#section-obtaining-verification-status"> for detailed API documentation</a>.
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="statusParams"></param>
        /// <returns></returns>
        public TeleSignResponse Status(string externalId, Dictionary<string, string> statusParams = null) {
            if (null == statusParams)
                statusParams = new Dictionary<string, string>();

            statusParams.Add("external_id", externalId);

            string resource = string.Format(AUTOVERIFY_STATUS_RESOURCE, externalId);            

            return Get(resource, statusParams);
        }
    }
}
