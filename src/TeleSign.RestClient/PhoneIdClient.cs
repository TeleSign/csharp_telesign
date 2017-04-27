using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace TeleSign.RestClient
{
    /// <summary>
    /// A set of APIs that deliver deep phone number data attributes 
    /// that help optimize the end user verification process and evaluate risk.
    /// </summary>
    public class PhoneIdClient : TeleSignRestClient
    {
        private const string PHONEID_RESOURCE = "/v1/phoneid/{0}";

        /// <summary>
        /// Initializes a new instance of the PhoneIdService class with a supplied credential and uri.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        public PhoneIdClient(string customerId, string apiKey, string restEndPoint, WebProxy proxy = null, string httpProxyUsername = null, string httpProxyPassword = null) : base(customerId, apiKey, restEndPoint, proxy, httpProxyUsername, httpProxyPassword) { }

        /// <summary>
        /// The PhoneID API provides a cleansed phone number, phone type, 
        /// and telecom carrier information to determine the best communication method - SMS or voice. 
        /// See https://developer.telesign.com/docs/phoneid-api for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="phoneidParams"></param>
        /// <returns></returns>
        public TeleSignResponse PhoneId(string phoneNumber, Dictionary<String, String> phoneidParams = null) {
            
            string resource = string.Format(
                        CultureInfo.InvariantCulture,
                        PhoneIdClient.PHONEID_RESOURCE,
                        phoneNumber);

            return Post(resource, phoneidParams);
        }
    }
}
