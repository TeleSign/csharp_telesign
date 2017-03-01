using System;
using System.Collections.Generic;
using System.Net;
namespace TeleSign.Services.Voice
{
    public class RawVoiceService : TeleSignService
    {
        private const string VOICE_RESOURCE = "/v1/voice";
        private const string VOICE_STATUS_RESOURCE = "/v1/voice/{0}";

        public RawVoiceService(TeleSignServiceConfiguration configuration) :base(configuration, null) { }

        public TSResponse CallRaw(string phoneNumber, string message, string messageType, Dictionary<string, string> callParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            if (null == callParams)
                callParams = new Dictionary<string, string>();

            callParams.Add("phone_number", phoneNumber);
            callParams.Add("message", message);
            callParams.Add("message_type", messageType);

            WebRequest request = this.ConstructWebRequest(VOICE_RESOURCE, "POST", callParams, AuthenticationMethod.HmacSha256);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        public TSResponse StatusRaw(string referenceId, Dictionary<String, String> statusParams = null)
        {
            CheckArgument.NotNullOrEmpty(referenceId, "reference_id");
            if (null == statusParams)
                statusParams = new Dictionary<string, string>();
            statusParams.Add("reference_id", referenceId);

            string resourceName = string.Format(VOICE_STATUS_RESOURCE, referenceId);

            WebRequest request = this.ConstructWebRequest(resourceName, "GET", statusParams, AuthenticationMethod.HmacSha256);

            return this.WebRequester.ReadTeleSignResponse(request);

        }
    }
}
