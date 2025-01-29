using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Telesign
{
    public class MessagingClient : RestClient
    {
        private const string MESSAGING_RESOURCE = "/v1/messaging";
        private const string MESSAGING_STATUS_RESOURCE = "/v1/messaging/{0}";

        public MessagingClient(string customerId,
                                string apiKey)
            : base(customerId,
                   apiKey)
        { }

        public MessagingClient(string customerId,
                                string apiKey,
                                string restEndPoint)
            : base(customerId,
                   apiKey,
                   restEndPoint)
        { }

        public MessagingClient(string customerId,
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
        /// Send a message to the target phone_number.
        /// 
        /// See https://developer.telesign.com/docs/messaging-api for detailed API documentation.
        /// </summary>
        public TelesignResponse Message(string phoneNumber, string message, string messageType, Dictionary<string, string> parameters = null)
        {
            if (null == parameters)
                parameters = new Dictionary<string, string>();

            parameters.Add("phone_number", phoneNumber);
            parameters.Add("message", message);
            parameters.Add("message_type", messageType);

            return Post(MESSAGING_RESOURCE, parameters);
        }

        /// <summary>
        /// Retrieves the current status of the message.
        /// 
        /// See https://developer.telesign.com/docs/messaging-api for detailed API documentation.
        /// </summary>
        public TelesignResponse Status(string referenceId, Dictionary<string, string> parameters = null)
        {
            return Get(string.Format(MESSAGING_STATUS_RESOURCE, referenceId), parameters);
        }

        /// <summary>
        /// Send a message to the target phone_number.
        /// 
        /// See https://developer.telesign.com/docs/messaging-api for detailed API documentation.
        /// </summary>
        public Task<TelesignResponse> MessageAsync(string phoneNumber, string message, string messageType, Dictionary<string, string> parameters = null)
        {
            if (null == parameters)
                parameters = new Dictionary<string, string>();

            parameters.Add("phone_number", phoneNumber);
            parameters.Add("message", message);
            parameters.Add("message_type", messageType);

            return PostAsync(MESSAGING_RESOURCE, parameters);
        }

        /// <summary>
        /// Retrieves the current status of the message.
        /// 
        /// See https://developer.telesign.com/docs/messaging-api for detailed API documentation.
        /// </summary>
        public Task<TelesignResponse> StatusAsync(string referenceId, Dictionary<string, string> parameters = null)
        {
            return GetAsync(string.Format(MESSAGING_STATUS_RESOURCE, referenceId), parameters);
        }
    }

}