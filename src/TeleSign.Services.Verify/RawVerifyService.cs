﻿//-----------------------------------------------------------------------
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
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    /// <para>
    /// The raw TeleSign Verify service. This class builds and makes requests
    /// to the TeleSign service and returns the raw JSON responses that the
    /// REST service returns.
    /// </para>
    /// <para>
    /// In most cases you should use VerifyService class instead which will
    /// parse the JSON responses into .NET objects that you can use.
    /// </para>
    /// </summary>
    public class RawVerifyService : TeleSignService
    {
        /// <summary>
        /// Format string for Verify request uri. The sub resource sms|call is
        /// filled into the format field when initiating verify requests and
        /// the reference id is used here for status requests.
        /// </summary>
        private const string VerifyResourceFormatString = "/v1/verify/{0}";

        /// <summary>
        /// Initializes a new instance of the RawVerifyService class supplying the 
        /// credential and uri to be used.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        public RawVerifyService(TeleSignServiceConfiguration configuration)
            : base(configuration, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RawVerifyService class with a supplied credential and uri and
        /// a web requester. In general you do not need to use this constructor unless you want to intercept
        /// the web requests for logging/debugging/testing purposes.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        /// <param name="webRequester">The web requester to use.</param>
        public RawVerifyService(
                    TeleSignServiceConfiguration configuration, 
                    IWebRequester webRequester)
            : base(configuration, webRequester)
        {
        }

        /// <summary>
        /// Initiates a PhoneId SMS transaction returning the raw JSON response from
        /// the REST API.
        /// </summary>
        /// <param name="phoneNumber">The phone number to send the sms to.</param>
        /// <param name="verifyCode">
        /// The code to send to the user. When null a code will
        /// be generated for you.
        /// </param>
        /// <param name="messageTemplate">
        /// A template for the message to be sent to the user.
        /// </param>
        /// <param name="language">
        /// The language that the message should be in. This parameter is ignored if
        /// you supplied a message template.
        /// </param>
        /// <returns>The raw JSON response from the REST API.</returns>
        public string SmsRaw(
                    string phoneNumber,
                    string verifyCode = null,
                    string messageTemplate = null,
                    string language = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            return this.InternalVerify(
                        VerificationMethod.Sms,
                        phoneNumber,
                        verifyCode,
                        messageTemplate,
                        language);
        }

        /// <summary>
        /// Initiates a PhoneId Voice Call transaction returning the raw JSON response from
        /// the REST API.
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
        /// <returns>The raw JSON response from the REST API.</returns>
        public string CallRaw(
                    string phoneNumber,
                    string verifyCode = null,
                    string language = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            return this.InternalVerify(
                        VerificationMethod.Call,
                        phoneNumber,
                        verifyCode,
                        null,
                        language);
        }
        
        /// <summary>
        /// The TeleSign Verify Push web service is a server-side component of the TeleSign AuthID application, and it allows you to provide on-device transaction authorization for your users. It works by delivering authorization requests to your users via Push Notification, and then by receiving their permission responses via their mobile device's wireless Internet connection. The service provides two levels of security to support two types of transactions.
        /// </summary>
        /// <param name="phoneNumber">The phone number to call.</param>
        /// <param name="notificationType">
        /// Indicates the security measure to use for transaction authorization. The following types are currently supported: CODE, SIMPLE
        /// </param>
        /// <param name="notificationValue">
        /// Applies when notification_type=CODE. The verification code used for the code challenge. This is a randomly generated numeric value that you display in your web page for your user. After reading this value, the user then types it into the AuthID application on their mobile device
        /// </param>
        /// <param name="template">
        /// Specifies a custom banner and icon for the TeleSign AuthID application to use for this notification. This allows you to brand your notifications with your corporate logo and/or your service-specific branding.
        /// </param>
        /// <param name="message">
        /// The message to display to the end user, in the body of the notification. You normally leave this parameter empty (or set to null), and the default message is displayed.
        /// </param>
        /// <returns>The raw JSON response from the REST API.</returns>
        public string PushRaw(
                    string phoneNumber,
                    string notificationType = null,
                    string notificationValue = null,
                    string template = null,
                    string message = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);
            
            if (notificationType == null)
            {
                notificationType = string.Empty;
            }
            
            if (notificationValue == null)
            {
                notificationValue = string.Empty;
            }

            if (template == null)
            {
                template = string.Empty;
            }

            if (message == null)
            {
                message = string.Empty;
            }

            Dictionary<string, string> args = ConstructVerifyArgs(
                        VerificationMethod.Push, 
                        phoneNumber, 
                        notificationType, 
                        notificationValue, 
                        template,
                        message);

            string resourceName = string.Format(
                        RawVerifyService.VerifyResourceFormatString, 
                        VerificationMethod.Push.ToString().ToLowerInvariant());

            WebRequest request = this.ConstructWebMobileRequest(
                        resourceName,
                        "POST",
                        args);

            return this.WebRequester.ReadResponseAsString(request);
        }
        
        /// <summary>
        /// The TeleSign Verify Soft Token web service is a server-side component of the TeleSign AuthID application, and it allows you to authenticate your end users when they use the TeleSign AuthID application on their mobile device to generate a Time-based One-time Password (TOTP) verification code
        /// </summary>
        /// <param name="phoneNumber">The phone number for the Verify Soft Token request, including country code</param>
        /// <param name="softTokenId">
        /// The alphanumeric string that uniquely identifies your TeleSign soft token subscription
        /// </param>
        /// <param name="verifyCode">
        /// The verification code received from the end user
        /// </param>
        /// <returns>The raw JSON response from the REST API.</returns>
        public string SoftTokenRaw(
                    string phoneNumber,
                    string softTokenId = null,
                    string verifyCode = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);
            
            if (softTokenId == null)
            {
                softTokenId = string.Empty;
            }
            
            if (verifyCode == null)
            {
                verifyCode = string.Empty;
            }

            Dictionary<string, string> args = ConstructVerifyArgs(
                        VerificationMethod.SoftToken,
                        phoneNumber
                        softTokenId, 
                        verifyCode);

            string resourceName = string.Format(
                        RawVerifyService.VerifyResourceFormatString, 
                        VerificationMethod.SoftToken.ToString().ToLowerInvariant());

            WebRequest request = this.ConstructWebMobileRequest(
                        resourceName,
                        "POST",
                        args);

            return this.WebRequester.ReadResponseAsString(request);
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
        public string TwoWaySmsRaw(
                    string phoneNumber,
                    string ucid,
                    string message = null,
                    string validityPeriod = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);
            
            if (message == null)
            {
                message = string.Empty;
            }
            
            if (validityPeriod == null)
            {
                validityPeriod = string.Empty;
            }

            Dictionary<string, string> args = ConstructVerifyArgs(
                        VerificationMethod.TwoWaySms,
                        phoneNumber,
                        ucid,
                        message,
                        validityPeriod);

            string resourceName = string.Format(
                        RawVerifyService.VerifyResourceFormatString, 
                        VerificationMethod.TwoWaySms.ToString().ToLowerInvariant());

            WebRequest request = this.ConstructWebRequest(
                        resourceName,
                        "POST",
                        args);

            return this.WebRequester.ReadResponseAsString(request);
        }

        /// <summary>
        /// Checks the status of a Verify transaction. When the code is
        /// supplied it verifies this code. The response also contains 
        /// status information about the progress of the call or sms.
        /// </summary>
        /// <param name="referenceId">
        /// The reference id return in the VerifyResponse from
        /// a call to Sms or Call.
        /// </param>
        /// <param name="verifyCode">The code the user provided you to be verified.</param>
        /// <returns>
        /// A VerifyResponse object containing information about the status
        /// of the transactation and validity of the code.
        /// </returns>
        public string StatusRaw(
                    string referenceId,
                    string verifyCode = null)
        {
            CheckArgument.NotNullOrEmpty(referenceId, "referenceId");

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("referenceId", referenceId);

            if (!string.IsNullOrEmpty(verifyCode))
            {
                args.Add("verify_code", verifyCode.ToString());
            }

            string resourceName = string.Format(
                        RawVerifyService.VerifyResourceFormatString, 
                        referenceId);

            WebRequest request = this.ConstructWebRequest(
                        resourceName,
                        "GET",
                        args);

            return this.WebRequester.ReadResponseAsString(request);
        }

        /// <summary>
        /// Common method for Verify SMS and call requests.
        /// </summary>
        /// <param name="verificationMethod">The verification method (call or sms).</param>
        /// <param name="phoneNumber">The phone number to send verification to.</param>
        /// <param name="verifyCode">
        /// The code to send to the user. When null a code will
        /// be generated for you.
        /// </param>
        /// <param name="messageTemplate">
        /// A template for the message to be sent to the user. This is ignored for voice calls.
        /// </param>
        /// <param name="language">
        /// The language that the message should be in. This parameter is ignored for
        /// SMS if you supplied a message template.
        /// </param>
        /// <returns>The raw JSON response for a verify REST API call.</returns>
        private string InternalVerify(
                    VerificationMethod verificationMethod,
                    string phoneNumber,
                    string verifyCode = null,
                    string messageTemplate = null,
                    string language = null)
        {
            this.CleanupPhoneNumber(phoneNumber);

            if (messageTemplate == null)
            {
                messageTemplate = string.Empty;
            }

            if (string.IsNullOrEmpty(language))
            {
                language = "en";
            }

            Dictionary<string, string> args = ConstructVerifyArgs(
                        verificationMethod, 
                        phoneNumber, 
                        verifyCode, 
                        messageTemplate, 
                        language);

            string resourceName = string.Format(
                        RawVerifyService.VerifyResourceFormatString, 
                        verificationMethod.ToString().ToLowerInvariant());

            WebRequest request = this.ConstructWebRequest(
                        resourceName,
                        "POST",
                        args);

            return this.WebRequester.ReadResponseAsString(request);
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
                    string language)
        {
            // TODO: Review code generation rules.
            if (verifyCode == null)
            {
                Random r = new Random();
                verifyCode = r.Next(100, 99999).ToString();
            }

            // TODO: Check code validity here?

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("phone_number", phoneNumber);
            args.Add("verify_code", verifyCode.ToString());
            args.Add("language", language);

            if (verificationMethod == VerificationMethod.Sms)
            {
                args.Add("template", messageTemplate);
            }

            return args;
        }
    }
}
