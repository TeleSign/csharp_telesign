using System;
using System.Collections.Generic;
using System.Net;

namespace TeleSign.Services.Voice
{
    public class VoiceClient : TeleSignService
    {
        private const string VOICE_RESOURCE = "/v1/voice";
        private const string VOICE_STATUS_RESOURCE = "/v1/voice/{0}";

        public VoiceClient(TeleSignServiceConfiguration configuration = null) : base(configuration, null) { }
        /// <summary>
        /// Initializes a new instance of the VoiceClient class with a supplied credential and uri and
        /// a web requester. In general you do not need to use this constructor unless you want to intercept
        /// the web requests for logging/debugging/testing purposes.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        /// <param name="webRequester">The web requester to use.</param>
        public VoiceClient(
                    TeleSignServiceConfiguration configuration,
                    IWebRequester webRequester)
            : base(configuration, webRequester)
        {
        }

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

        /// <summary>
        /// Retrieves the current status of the voice call. See https://developer.telesign.com/docs/voice-api for detailed API documentation.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="statusParams"></param>
        /// <returns></returns>
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