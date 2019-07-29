using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Telesign
{
    /// <summary>
    ///  AppVerify is a secure, lightweight SDK that integrates a frictionless user verification process into existing native mobile applications.
    /// </summary>
    public class AppVerifyClient : RestClient
    {
        private const string APPVERIFY_STATUS_RESOURCE = "/v1/mobile/verification/status/{0}";

        public AppVerifyClient(string customerId,
                                string apiKey)
            : base(customerId,
                   apiKey)
        { }

        public AppVerifyClient(string customerId,
                                string apiKey,
                                string restEndPoint)
            : base(customerId,
                   apiKey,
                   restEndPoint)
        { }

        public AppVerifyClient(string customerId,
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
        {
        }

        /// <summary>
        /// Retrieves the verification result for an AppVerify transaction by externalId. To ensure a secure verification
        /// flow you must check the status using TeleSign's servers on your backend. Do not rely on the SDK alone to
        /// indicate a successful verification.
        ///
        /// See https://developer.telesign.com/docs/app-verify-android-sdk-self#section-obtaining-verification-status or
        /// https://developer.telesign.com/docs/app-verify-ios-sdk-self#section-obtaining-verification-status for detailed
        /// API documentation.
        /// </summary>
        public TelesignResponse Status(string externalId, Dictionary<string, string> parameters = null)
        {
            return this.Get(string.Format(APPVERIFY_STATUS_RESOURCE, externalId), parameters);
        }

        /// <summary>
        /// Retrieves the verification result for an AppVerify transaction by externalId. To ensure a secure verification
        /// flow you must check the status using TeleSign's servers on your backend. Do not rely on the SDK alone to
        /// indicate a successful verification.
        ///
        /// See https://developer.telesign.com/docs/app-verify-android-sdk-self#section-obtaining-verification-status or
        /// https://developer.telesign.com/docs/app-verify-ios-sdk-self#section-obtaining-verification-status for detailed
        /// API documentation.
        /// </summary>
        public Task<TelesignResponse> StatusAsync(string externalId, Dictionary<string, string> parameters = null)
        {
            return this.GetAsync(string.Format(APPVERIFY_STATUS_RESOURCE, externalId), parameters);
        }
    }
}
