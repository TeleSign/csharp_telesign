using System;
using System.Collections.Generic;
using System.Net;
namespace TeleSign.Services.Voice
{
    public class VoiceClient : TeleSignService
    {
        private const string VOICE_RESOURCE = "/v1/voice";
        private const string VOICE_STATUS_RESOURCE = "/v1/voice/{0}";

        public VoiceClient(TeleSignServiceConfiguration configuration) :base(configuration, null) { }

        /// <summary>
        /// Send a voice call to the target phone_number. See https://developer.telesign.com/docs/voice-api for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="callParams"></param>
        /// <returns></returns>
        public TeleSignResponse Call(string phoneNumber, string message, string messageType, Dictionary<string, string> callParams = null)
        {
            if (null == callParams)
                callParams = new Dictionary<string, string>();

            callParams.Add("phone_number", phoneNumber);
            callParams.Add("message", message);
            callParams.Add("message_type", messageType);

            WebRequest request = this.ConstructWebRequest(VOICE_RESOURCE, "POST", callParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        public TeleSignResponse Status(string referenceId, Dictionary<String, String> statusParams = null)
        {            
            if (null == statusParams)
                statusParams = new Dictionary<string, string>();
            statusParams.Add("reference_id", referenceId);

            string resourceName = string.Format(VOICE_STATUS_RESOURCE, referenceId);

            WebRequest request = this.ConstructWebRequest(resourceName, "GET", statusParams);

            return this.WebRequester.ReadTeleSignResponse(request);

        }
    }
}
