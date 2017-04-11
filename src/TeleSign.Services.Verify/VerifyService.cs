//-----------------------------------------------------------------------
// <copyright file="VerifyService.cs" company="TeleSign Corporation">
//     Copyright (c) TeleSign Corporation 2012.
// </copyright>
// <license>MIT</license>
// <author>Zentaro Kavanagh</author>
//-----------------------------------------------------------------------

namespace TeleSign.Services.Verify
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The TeleSign Verify service.
    /// </summary>
    public class VerifyService : RawVerifyService
    {
        /// <summary>
        /// The parser used to transform the raw JSON string responses to rich .NET objects.
        /// No Longer Needed
        /// </summary>
        //private IVerifyResponseParser parser;

        /// <summary>
        /// Initializes a new instance of the VerifyService class with a supplied credential and URI.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        public VerifyService(TeleSignServiceConfiguration configuration)
            : this(
                        configuration, 
                        null)                        
        {
        }

        /// <summary>
        /// Initializes a new instance of the VerifyService class with a supplied credential and uri and
        /// a web requester. In general you do not need to use this constructor unless you want to intercept
        /// the web requests for logging/debugging/testing purposes.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        /// <param name="webRequester">The web requester to use.</param>        
        public VerifyService(
                    TeleSignServiceConfiguration configuration, 
                    IWebRequester webRequester)                    
            : base(configuration, webRequester)
        {
            //this.parser = responseParser;
        }


        /// <summary>
        /// The SMS Verify API delivers phone-based verification and two-factor authentication using a time-based, one-time passcode sent over SMS. See https://developer.telesign.com/docs/rest_api-verify-sms for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="smsParams"></param>
        /// <returns></returns>
        public TSResponse SendSms(
                    string phoneNumber,
                    Dictionary<string, string> smsParams = null)
        {
            TSResponse response = new TSResponse();
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);
            //this.ValidateCodeFormat(verifyCode);

            try
            {
                response = this.SmsRaw(
                        phoneNumber,
                        smsParams);
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing Verify SMS response",
                            response.ToString(),
                            x);
            }
            return response;
        }       
        
        /// <summary>
        /// The TeleSign Verify 2-Way SMS web service allows you to authenticate your users and verify user transactions via two-way Short Message Service (SMS) wireless communication. Verification requests are sent to user’s in a text message, and users return their verification responses by replying to the text message.
        /// </summary>
        /// <param name="phoneNumber">The phone number for the Verify Soft Token request, including country code</param>
        /// <param name="ucid">
        /// A string specifying one of the Use Case Codes
        /// </param>
        /// <param name="message">
        /// The text to display in the body of the text message. You must include the $$CODE$$ placeholder for the verification code somewhere in your message text. TeleSign automatically replaces it with a randomly-generated verification code
        /// </param>
        /// <param name="validityPeriod">
        /// This parameter allows you to place a time-limit on the verification. This provides an extra level of security by restricting the amount of time your end user has to respond (after which, TeleSign automatically rejects their response). Values are expressed as a natural number followed by a lower-case letter that represents the unit of measure. You can use 's' for seconds, 'm' for minutes, 'h' for hours, and 'd' for days
        /// </param>
        /// <returns>The raw JSON response from the REST API.</returns>
        public TSResponse SendTwoWaySms(
        			string phoneNumber,
                    string message = null,
            		string validityPeriod = "5m")
        {
            TSResponse response = new TSResponse();
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);


            try
            {
            response = this.TwoWaySmsRaw(
                        phoneNumber,
                        message,
            			validityPeriod);
                return response;
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing Verify TwoWaySms response",
                            response.ToString(),
                            x);
            }
        }

        /// <summary>
        /// Initiates a TeleSign Verify transaction via a voice call.
        /// </summary>
        /// <param name="phoneNumber">The phone number to call.</param>
        /// <param name="verifyCode">
        /// The code to send to the user. When null a code will
        /// be generated for you.
        /// </param>
        /// <param name="language">
        /// The language that the message should be in. This parameter is ignored if
        /// you supplied a message template.
        /// TODO: Details about language string format.
        /// </param>
        /// <returns>
        /// A VerifyResponse object with the status and returned information
        /// for the transaction.
        /// </returns>
        public TSResponse Voice(
                    string phoneNumber,
                    Dictionary<string, string> callParams = null)
        {
            TSResponse response = new TSResponse();
            try
            {
                response = this.CallRaw(
                        phoneNumber,
                        callParams);
                return response;
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing Verify call response",
                            response.ToString(),
                            x);
            }
        }

        /// <summary>
        /// Initiates a TeleSign Verify transaction via a voice call.
        /// </summary>
        /// <param name="phoneNumber">The phone number to call.</param>
        /// <param name="verifyCode">
        /// The code to send to the user. When null a code will
        /// be generated for you.
        /// </param>
        /// <param name="language">
        /// The language that the message should be in. This parameter is ignored if
        /// you supplied a message template.
        /// </param>
        /// <returns>
        /// A VerifyResponse object with the status and returned information
        /// for the transaction.
        /// </returns>
        public TSResponse InitiatePush(
                    string phoneNumber,
                    Dictionary<string, string> pushParams = null)
        {
            TSResponse response = new TSResponse();

            try
            {
                response = this.PushRaw(
                        phoneNumber,
                        pushParams);

                return response;
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing Verify call response",
                            response.ToString(),
                            x);
            }
        }
        
        /// <summary>
        /// Retrieves the verification result for any verify resource. See https://developer.telesign.com/docs/rest_api-verify-transaction-callback for detailed API documentation.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="statusParams"></param>
        /// <returns></returns>
        public TSResponse CheckStatus(string referenceId, Dictionary<string, string> statusParams = null)
        {
            CheckArgument.NotNullOrEmpty(referenceId, "referenceId");

            TSResponse response = new TSResponse();           

            try
            {
                response = this.StatusRaw(referenceId, statusParams);
                return response;
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing Verify code check status response",
                            response.ToString(),
                            x);
            }
        }

        /// <summary>
        /// The Smart Verify web service simplifies the process of verifying user identity by integrating several TeleSign
        /// web services into a single API call. This eliminates the need for you to make multiple calls to the TeleSign
        /// Verify resource.
        /// See https://developer.telesign.com/docs/rest_api-smart-verify for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="ucid"></param>
        /// <param name="smartParams"></param>
        /// <returns>The TeleSign JSON response from the REST API.</returns>
        public TSResponse Smart(
                    string phoneNumber,
                    string ucid,
                    Dictionary<string, string> smartParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            TSResponse response = new TSResponse();

            try
            {
                response = this.SmartRaw(phoneNumber, ucid, smartParams);
                return response;
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing Verify - Smart response",
                            response.ToString(),
                            x);
            }
        }

        /// <summary>
        /// Validates whether a verifyCode is valid to use with the TeleSign API.
        /// Null is valid and indicates the API should generate the code itself,
        /// empty strings are not valid, any non-digit characters are not valid
        /// and leading zeros are not valid.
        /// </summary>
        /// <param name="verifyCode">The code to verify.</param>
        public virtual void ValidateCodeFormat(string verifyCode)
        {
            if (verifyCode == null)
            {
                // When the code is null we generate it, so this is valid.
                return;
            }

            // Empty code is never valid
            CheckArgument.NotEmpty(verifyCode, "verifyCode");

            foreach (char c in verifyCode)
            {
                // Only decimal digits are allowed 0-9.
                if (!Char.IsDigit(c))
                {
                    string message = string.Format(
                                "Verify code '{0}' must only contain decimal digits [0-9]",
                                verifyCode);

                    throw new ArgumentException(message);
                }
            }
        }
    }
}
