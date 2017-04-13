//-----------------------------------------------------------------------
// <copyright file="RawVerifyService.cs" company="TeleSign Corporation">
//     Copyright (c) TeleSign Corporation 2012.
// </copyright>
// <license>MIT</license>
// <author>Zentaro Kavanagh</author>
//-----------------------------------------------------------------------

namespace TeleSign.Services.Verify
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// The TeleSign Verify service.
    /// </summary>
    public class VerifyService : TeleSignService
    {
        /// <summary>
        /// Format string for Verify request uri. The sub resource sms|call is
        /// filled into the format field when initiating verify requests and
        /// the reference id is used here for status requests.
        /// </summary>
        private const string VerifyResourceFormatString = "/v1/verify/{0}";

        /// <summary>
        /// Initializes a new instance of the VerifyService class supplying the 
        /// credential and uri to be used.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        public VerifyService(TeleSignServiceConfiguration configuration)
            : base(configuration, null)
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
        }
        
        /// <summary>
        /// The SMS Verify API delivers phone-based verification and two-factor authentication using a time-based,
        /// one-time passcode sent over SMS.
        /// See https://developer.telesign.com/docs/rest_api-verify-sms for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="smsParams"></param>
        /// <returns></returns>
        public TeleSignResponse SendSms(
                    string phoneNumber,
                    Dictionary<string, string> smsParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            return this.InternalVerify(
                        VerificationMethod.Sms,
                        phoneNumber,
                        smsParams);
        }


        /// <summary>
        /// The Voice Verify API delivers patented phone-based verification and two-factor authentication using a one-time
        /// passcode sent over voice message.
        ///  See https://developer.telesign.com/docs/rest_api-verify-call for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber">The phone number to call.</param>
        /// <param name="callParams"></param>
        /// <returns>The TeleSign JSON response from the REST API.</returns>
        public TeleSignResponse Voice(
                    string phoneNumber,
                    Dictionary<string, string> callParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            return this.InternalVerify(
                        VerificationMethod.Call,
                        phoneNumber,
                        callParams);
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
        public TeleSignResponse Smart(
                    string phoneNumber,
                    string ucid,
                    Dictionary<string, string> smartParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            return this.InternalVerify(
                        VerificationMethod.Smart,
                        phoneNumber,
                        smartParams);
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
        public TeleSignResponse TwoWaySms(
                    string phoneNumber,
                    string message,
            		string validityPeriod = "5m",
                    string useCaseId = VerifyService.DefaultUseCaseId)
        {
            CheckArgument.NotEmpty(message, "message");
            CheckArgument.NotNullOrEmpty(validityPeriod, "validityPeriod");

            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            Dictionary<string, string> args = ConstructVerifyArgs(
                        VerificationMethod.TwoWaySms,
                		phoneNumber,
                		null,
                        message,
                        null,
                        validityPeriod,
                        useCaseId);

            string resourceName = string.Format(
                        VerifyService.VerifyResourceFormatString, 
                        "two_way_sms");

            WebRequest request = this.ConstructWebRequest(
                        resourceName,
                        "POST",
                        args);

            return this.WebRequester.ReadTeleSignResponse(request);
        }


        /// <summary>
        /// The Push Verify web service allows you to provide on-device transaction authorization for your end users. It
        /// works by delivering authorization requests to your end users via push notification, and then by receiving their
        /// permission responses via their mobile device's wireless Internet connection.
        /// See https://developer.telesign.com/docs/rest_api-verify-push for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="pushParams"></param>
        /// <returns>The TeleSign JSON response from the REST API.</returns>
        public TeleSignResponse Push(
                    string phoneNumber,
                    Dictionary<string, string> pushParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            return this.InternalVerify(
                        VerificationMethod.Push,
                        phoneNumber,
                        pushParams);
        }

        /// <summary>
        /// Retrieves the verification result for any verify resource.
        /// See https://developer.telesign.com/docs/rest_api-verify-transaction-callback for detailed API documentation.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="statusParams"></param>
        /// <returns></returns>
        public TeleSignResponse CheckStatus(
                    string referenceId,
                    Dictionary<string, string> statusParams = null)
        {
            CheckArgument.NotNullOrEmpty(referenceId, "referenceId");
            if(null == statusParams)
                statusParams = new Dictionary<string, string>();
            statusParams.Add("referenceId", referenceId);            

            string resourceName = string.Format(
                        VerifyService.VerifyResourceFormatString, 
                        referenceId);

            WebRequest request = this.ConstructWebRequest(
                        resourceName,
                        "GET",
                        statusParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        /// <summary>
        /// Common method for Verify SMS and call requests.
        /// </summary>
        /// <param name="verificationMethod">The verification method (call or sms).</param>
        /// <param name="phoneNumber">The phone number to send verification to.</param>
        /// <param name="verifyParams"></param>
        /// <returns>TeleSign response as a Object composed of JSON.</returns>
        private TeleSignResponse InternalVerify(VerificationMethod verificationMethod, string phoneNumber, Dictionary<string, string> verifyParams)
        {
            this.CleanupPhoneNumber(phoneNumber);

            if (null == verifyParams)
                verifyParams = new Dictionary<string, string>();

            verifyParams.Add("phone_number", phoneNumber);

            string resourceName = string.Format(
                        VerifyService.VerifyResourceFormatString,
                        verificationMethod.ToString().ToLowerInvariant());

            WebRequest request = this.ConstructWebRequest(
                        resourceName,
                        "POST",
                        verifyParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        /// <summary>
        /// Constructs the arguments for a Verify transaction.
        /// </summary>
        /// <param name="verificationMethod">The method of verification (sms or call).</param>
        /// <param name="phoneNumber">The phone number to send verification to.</param>
        /// <param name="verifyCode">The code to be sent to the user. If null - a code will be generated.</param>
        /// <param name="messageTemplate">An optional template for the message. Ignored if not SMS.</param>
        /// <param name="language">The language for the message. Ignored for SMS when a template is provided.</param>
        /// <returns>A dictionary of arguments for a Verify transaction.</returns>
        private static Dictionary<string, string> ConstructVerifyArgs(
                    VerificationMethod verificationMethod, 
                    string phoneNumber, 
                    string verifyCode, 
                    string messageTemplate, 
                    string language,
                    string validityPeriod = null,
                    string useCaseId = VerifyService.DefaultUseCaseId)
        {
            // TODO: Review code generation rules.
            if (verifyCode == null)
            {
                Random r = new Random();
                verifyCode = r.Next(100, 99999).ToString();
            }
            else
            {
                if (verificationMethod == VerificationMethod.TwoWaySms)
                {
                    throw new ArgumentException("Verify Code cannot be specified for Two-Way SMS", "verifyCode");
                }
            }

            // TODO: Check code validity here?

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("phone_number", phoneNumber);

            if (verificationMethod == VerificationMethod.Push)
            {
                if (!string.IsNullOrEmpty(verifyCode))
                {
                    args.Add("notification_value", verifyCode.ToString());
                }
            }
            else if (verificationMethod == VerificationMethod.TwoWaySms)
            {
                // Two way sms doesn't take a verify code. So nothing here.
            }
            else
            {
                args.Add("verify_code", verifyCode.ToString());
            }

            if (!string.IsNullOrEmpty(language))
            {
                args.Add("language", language);
            }

            if (verificationMethod == VerificationMethod.Sms || verificationMethod == VerificationMethod.Push)
            {
                args.Add("template", messageTemplate);
            }

            if (verificationMethod == VerificationMethod.TwoWaySms)
            {
                args.Add("message", messageTemplate);
                args.Add("validity_period", validityPeriod);
                args.Add("ucid", useCaseId);
            }

            return args;
        }
    }
}
