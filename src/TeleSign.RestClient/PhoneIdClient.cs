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
        
        public PhoneIdClient(string customerId, string apiKey, string restEndPoint, int timeout = 10000, int readWriteTimeout = 10000, WebProxy proxy = null, string httpProxyUsername = null, string httpProxyPassword = null) : base(customerId, apiKey, restEndPoint, timeout, readWriteTimeout, proxy, httpProxyUsername, httpProxyPassword) { }

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
                        PHONEID_RESOURCE,
                        phoneNumber);

            return Post(resource, phoneidParams);
        }
    }
}
