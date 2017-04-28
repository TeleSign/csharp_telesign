using System;
using System.Collections.Generic;
using System.Net;

namespace TeleSign.RestClient
{
    class MessagingClient : TeleSignRestClient
    {
        private const string MESSAGING_RESOURCE = "/v1/messaging";
        private const string MESSAGING_STATUS_RESOURCE = "/v1/messaging/{0}";

        public MessagingClient(string customerId, string apiKey, string restEndPoint, int timeout = 10000, int readWriteTimeout = 10000, WebProxy proxy = null , string httpProxyUsername = null, string httpProxyPassword = null) : base(customerId, apiKey, restEndPoint, timeout, readWriteTimeout, proxy, httpProxyUsername, httpProxyPassword) { }

        /// <summary>
        /// Send a message to the target phone_number.See <a href ="https://developer.telesign.com/v2.0/docs/messaging-api">for detailed API documentation</a>.         
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="messageParams"></param>
        /// <returns></returns>
        public TeleSignResponse Message(string phoneNumber, string message, string messageType, Dictionary<string, string> messageParams = null)
        {
            if (null == messageParams)
                messageParams = new Dictionary<string, string>();

            messageParams.Add("phone_number", phoneNumber);
            messageParams.Add("message", message);
            messageParams.Add("message_type", messageType);
                        
            return Post(MESSAGING_RESOURCE, messageParams);
        }

        /// <summary>
        /// Retrieves the current status of the message. See <a href="https://developer.telesign.com/v2.0/docs/messaging-api"> for detailed API documentation</a>.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="statusParams"></param>
        /// <returns></returns>
        public TeleSignResponse Status(string referenceId, Dictionary<string, string> statusParams = null)
        {
            if (null == statusParams)
                statusParams = new Dictionary<string, string>();
            statusParams.Add("reference_id", referenceId);

            string resourceName = string.Format(MESSAGING_STATUS_RESOURCE, referenceId);

            return Get(resourceName, statusParams);
        }
    }
}
