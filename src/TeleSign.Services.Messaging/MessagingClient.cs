using System;
using System.Collections.Generic;
using System.Net;
namespace TeleSign.Services.Messaging
{
    /// <summary>
    /// TeleSign's Messaging API allows you to easily send SMS messages. 
    /// You can send alerts, reminders, and notifications, or you can send verification messages containing one-time passcodes(OTP).
    /// </summary>
    public class MessagingClient : TeleSignService
    {
        private const String MESSAGING_RESOURCE = "/v1/messaging";
        private const String MESSAGING_STATUS_RESOURCE = "/v1/messaging/{0}";

        public MessagingClient(TeleSignServiceConfiguration configuration) :base(configuration, null) { }

        /// <summary>
        /// Send a message to the target phone_number.See <a href ="https://developer.telesign.com/v2.0/docs/messaging-api">for detailed API documentation</a>.         
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="messageParams"></param>
        /// <returns></returns>
        public TeleSignResponse Message(string phoneNumber, string message, string messageType, Dictionary<string, string> messageParams = null) {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            if (null == messageParams)
                messageParams = new Dictionary<string, string>();

            messageParams.Add("phone_number", phoneNumber);
            messageParams.Add("message", message);
            messageParams.Add("message_type", messageType);

            WebRequest request = this.ConstructWebRequest(MESSAGING_RESOURCE, "POST", messageParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        /// <summary>
        /// Retrieves the current status of the message. See <a href="https://developer.telesign.com/v2.0/docs/messaging-api"> for detailed API documentation</a>.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="statusParams"></param>
        /// <returns></returns>
        public TeleSignResponse Status(string referenceId, Dictionary<String, String> statusParams = null) {
            CheckArgument.NotNullOrEmpty(referenceId, "reference_id");
            if (null == statusParams)
                statusParams = new Dictionary<string, string>();
            statusParams.Add("reference_id", referenceId);

            string resourceName = string.Format(MESSAGING_STATUS_RESOURCE, referenceId);

            WebRequest request = this.ConstructWebRequest(resourceName, "GET", statusParams);

            return this.WebRequester.ReadTeleSignResponse(request);

        }
    }
}
