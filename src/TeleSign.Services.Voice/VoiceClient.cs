using System;
using System.Collections.Generic;

namespace TeleSign.Services.Voice
{
    public class VoiceClient:RawVoiceService
    {
        public VoiceClient(TeleSignServiceConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Send a voice call to the target phone_number. See https://developer.telesign.com/docs/voice-api for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="callParams"></param>
        /// <returns></returns>
        public TSResponse Call(string phoneNumber, string message, string messageType, Dictionary<string, string> callParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);
            TSResponse response = new TSResponse();
            try
            {
                response = this.CallRaw(phoneNumber, message, messageType, callParams);
            }
            catch (Exception e)
            {
                throw new ResponseParseException(
                            "Error parsing Call response",
                            response.ToString(),
                            e);
            }

            return response;
        }

        /// <summary>
        /// Retrieves the current status of the voice call. See https://developer.telesign.com/docs/voice-api for detailed API documentation.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="statusParams"></param>
        /// <returns></returns>
        public TSResponse Status(string referenceId, Dictionary<String, String> statusParams = null) {
            TSResponse response = new TSResponse();
            try
            {
                response = this.StatusRaw(referenceId, statusParams);
                return response;
            }
            catch (Exception e)
            {

                throw new ResponseParseException(
                            "Error parsing Status response",
                            response.ToString(),
                            e);
            }
        }
    }
}
