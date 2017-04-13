using System;
using System.Collections.Generic;

namespace TeleSign.Services.Messaging
{
    public class MessagingClient:RawMessagingService
    {
        public MessagingClient(TeleSignServiceConfiguration configuration): base(configuration) { }

        /// <summary>
        /// Send a message to the target phone_number.See <a href ="https://developer.telesign.com/v2.0/docs/messaging-api">for detailed API documentation</a>.         
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="messageParams"></param>
        /// <returns></returns>
        public TeleSignResponse Message(String phoneNumber, String message,
                String messageType, Dictionary<String, String> messageParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);
            TeleSignResponse response = new TeleSignResponse();
            try
            {
                response = this.MessageRaw(phoneNumber, message, messageType, messageParams);
            }
            catch (Exception e)
            {
                throw new ResponseParseException(
                            "Error parsing Message response",
                            response.ToString(),
                            e);
            }

            return response;
        }
   
        /// <summary>
        /// Retrieves the current status of the message. See <a href="https://developer.telesign.com/v2.0/docs/messaging-api"> for detailed API documentation</a>.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="statusParams"></param>
        /// <returns></returns>
        public TeleSignResponse Status(String referenceId,
                Dictionary<String, String> statusParams=null)
        {

            TeleSignResponse response = new TeleSignResponse();
            try
            {
                response = this.StatusRaw(referenceId, statusParams);
                return response;
            }
            catch (Exception e)
            {

                throw new ResponseParseException(
                            "Error parsing Message response",
                            response.ToString(),
                            e);
            }            
        }
    }
}
