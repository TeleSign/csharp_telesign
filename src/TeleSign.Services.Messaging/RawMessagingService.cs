using System;
using System.Collections.Generic;
using System.Net;
namespace TeleSign.Services.Messaging
{
    public class RawMessagingService : TeleSignService
    {
        private const String MESSAGING_RESOURCE = "/v1/messaging";
        private const String MESSAGING_STATUS_RESOURCE = "/v1/messaging/{0}";

        public RawMessagingService(TeleSignServiceConfiguration configuration) :base(configuration, null) { }

        public TeleSignResponse MessageRaw(string phoneNumber, string message, string messageType, Dictionary<string, string> messageParams = null) {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            if (null == messageParams)
                messageParams = new Dictionary<string, string>();

            messageParams.Add("phone_number", phoneNumber);
            messageParams.Add("message", message);
            messageParams.Add("message_type", messageType);

            WebRequest request = this.ConstructWebRequest(MESSAGING_RESOURCE, "POST", messageParams, AuthenticationMethod.HmacSha256);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        public TeleSignResponse StatusRaw(string referenceId, Dictionary<String, String> statusParams = null) {
            CheckArgument.NotNullOrEmpty(referenceId, "reference_id");
            if (null == statusParams)
                statusParams = new Dictionary<string, string>();
            statusParams.Add("reference_id", referenceId);

            string resourceName = string.Format(MESSAGING_STATUS_RESOURCE, referenceId);

            WebRequest request = this.ConstructWebRequest(resourceName, "GET", statusParams,AuthenticationMethod.HmacSha256);

            return this.WebRequester.ReadTeleSignResponse(request);

        }
    }
}
