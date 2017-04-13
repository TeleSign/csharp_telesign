using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace TeleSign.Services.PhoneId
{
    /// <summary>
    /// A set of APIs that deliver deep phone number data attributes 
    /// that help optimize the end user verification process and evaluate risk.
    /// </summary>
    public class PhoneIdClient : TeleSignService
    {
        private const string PHONEID_RESOURCE = "/v1/phoneid/{0}";

        /// <summary>
        /// Initializes a new instance of the PhoneIdService class with a supplied credential and uri.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        public PhoneIdClient(TeleSignServiceConfiguration configuration)
            : this(configuration, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PhoneIdService class with a supplied credential and uri and
        /// a web requester. In general you do not need to use this constructor unless you want to intercept
        /// the web requests for logging/debugging/testing purposes.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        /// <param name="webRequester">The web requester to use.</param>
        public PhoneIdClient(
                    TeleSignServiceConfiguration configuration,
                    IWebRequester webRequester)
            : base(configuration, webRequester)
        {
        }
        /// <summary>
        /// The PhoneID API provides a cleansed phone number, phone type, 
        /// and telecom carrier information to determine the best communication method - SMS or voice. 
        /// See https://developer.telesign.com/docs/phoneid-api for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="phoneidParams"></param>
        /// <returns></returns>
        public TeleSignResponse PhoneId(string phoneNumber, Dictionary<String, String> phoneidParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            string resourceName = string.Format(
                        CultureInfo.InvariantCulture,
                        PhoneIdClient.PHONEID_RESOURCE,
                        phoneNumber);

            WebRequest request = this.ConstructWebRequest(resourceName, "POST", phoneidParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }
    }
}
