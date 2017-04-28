using System;
using System.Collections.Generic;
using System.Net;

namespace TeleSign.RestClient
{
    public class VoiceClient : TeleSignRestClient
    {
        private const string VOICE_RESOURCE = "/v1/voice";
        private const string VOICE_STATUS_RESOURCE = "/v1/voice/{0}";

        public VoiceClient(string customerId, string apiKey, string restEndPoint, int timeout = 10000, int readWriteTimeout = 10000, WebProxy proxy = null, string httpProxyUsername = null, string httpProxyPassword = null) : base(customerId, apiKey, restEndPoint, timeout, readWriteTimeout, proxy, httpProxyUsername, httpProxyPassword) { }        

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

            return Post(VOICE_RESOURCE,callParams);
        }

        /// <summary>
        /// Retrieves the current status of the voice call. See https://developer.telesign.com/docs/voice-api for detailed API documentation.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="statusParams"></param>
        /// <returns></returns>
        public TeleSignResponse Status(string referenceId, Dictionary<string, string> statusParams = null)
        {
            if (null == statusParams)
                statusParams = new Dictionary<string, string>();
            statusParams.Add("reference_id", referenceId);

            string resource = string.Format(VOICE_STATUS_RESOURCE, referenceId);            

            return Get(resource, statusParams);
        }
    }
}