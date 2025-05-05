using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Telesign
{
    public class VoiceClient : RestClient
    {
        private const string VOICE_RESOURCE = "/v1/voice";
        private const string VOICE_STATUS_RESOURCE = "/v1/voice/{0}";

        public VoiceClient(string customerId,
                                string apiKey)
            : base(customerId,
                   apiKey)
        { }

        public VoiceClient(string customerId,
                                string apiKey,
                                string restEndPoint,
                                string source,
                                string sdkVersionOrigin,
                                string sdkVersionDependency)
            : base(customerId,
                   apiKey,
                   restEndPoint,
                   source: source,
                   sdkVersionOrigin: sdkVersionOrigin,
                   sdkVersionDependency: sdkVersionDependency)
        { }

        public VoiceClient(string customerId,
                                string apiKey,
                                string restEndPoint,
                                int timeout,
                                WebProxy proxy,
                                string proxyUsername,
                                string proxyPassword,
                                string source,
                                string sdkVersionOrigin,
                                string sdkVersionDependency)
            : base(customerId,
                   apiKey,
                   restEndPoint,
                   timeout,
                   proxy,
                   proxyUsername,
                   proxyPassword,
                   source,
                   sdkVersionOrigin,
                   sdkVersionDependency)
        { }

        /// <summary>
        /// Send a voice call to the target phone_number.
        /// 
        /// See https://developer.telesign.com/docs/voice-api for detailed API documentation.
        /// </summary>
        public TelesignResponse Call(string phoneNumber, string message, string messageType, Dictionary<string, string> callParams = null)
        {
            if (null == callParams)
                callParams = new Dictionary<string, string>();

            callParams.Add("phone_number", phoneNumber);
            callParams.Add("message", message);
            callParams.Add("message_type", messageType);

            return Post(VOICE_RESOURCE, callParams);
        }

        /// <summary>
        /// Retrieves the current status of the voice call.
        /// 
        /// See https://developer.telesign.com/docs/voice-api for detailed API documentation.
        /// </summary>
        public TelesignResponse Status(string referenceId, Dictionary<string, string> statusParams = null)
        {
            if (null == statusParams)
                statusParams = new Dictionary<string, string>();
            statusParams.Add("reference_id", referenceId);

            string resource = string.Format(VOICE_STATUS_RESOURCE, referenceId);

            return Get(resource, statusParams);
        }

        /// <summary>
        /// Send a voice call to the target phone_number.
        /// 
        /// See https://developer.telesign.com/docs/voice-api for detailed API documentation.
        /// </summary>
        public Task<TelesignResponse> CallAsync(string phoneNumber, string message, string messageType, Dictionary<string, string> callParams = null)
        {
            if (null == callParams)
                callParams = new Dictionary<string, string>();

            callParams.Add("phone_number", phoneNumber);
            callParams.Add("message", message);
            callParams.Add("message_type", messageType);

            return PostAsync(VOICE_RESOURCE, callParams);
        }

        /// <summary>
        /// Retrieves the current status of the voice call.
        /// 
        /// See https://developer.telesign.com/docs/voice-api for detailed API documentation.
        /// </summary>
        public Task<TelesignResponse> StatusAsync(string referenceId, Dictionary<string, string> statusParams = null)
        {
            if (null == statusParams)
                statusParams = new Dictionary<string, string>();
            statusParams.Add("reference_id", referenceId);

            string resource = string.Format(VOICE_STATUS_RESOURCE, referenceId);

            return GetAsync(resource, statusParams);
        }
    }

}