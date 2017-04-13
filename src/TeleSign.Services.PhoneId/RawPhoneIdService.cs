//-----------------------------------------------------------------------
// <copyright file="RawPhoneIdService.cs" company="TeleSign Corporation">
//     Copyright (c) TeleSign Corporation 2012.
// </copyright>
// <license>MIT</license>
// <author>Zentaro Kavanagh</author>
//-----------------------------------------------------------------------

namespace TeleSign.Services.PhoneId
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;

    /// <summary>
    /// <para>
    /// The raw TeleSign PhoneID service. This class builds and makes requests to the
    /// TeleSign service and returns the raw JSON responses that the REST service
    /// returns. 
    /// </para>
    /// <para>
    /// In most cases you should use PhoneIdService class instead which will parse the
    /// JSON responses into .NET objects that you can use. 
    /// </para>
    /// </summary>
    public class RawPhoneIdService : TeleSignService
    {
        /// <summary>
        /// Format string for PhoneId Standard request uri. The phone number is
        /// filled into the format field.
        /// </summary>
        private const string StandardResourceFormatString = "/v1/phoneid/standard/{0}";

        /// <summary>
        /// Format string for PhoneId Contact request uri. The phone number is
        /// filled into the format field.
        /// </summary>
        private const string ContactResourceFormatString = "/v1/phoneid/contact/{0}";

        /// <summary>
        /// Format string for PhoneId Score request uri. The phone number is
        /// filled into the format field.
        /// </summary>
        private const string ScoreResourceFormatString = "/v1/phoneid/score/{0}";

        /// <summary>
        /// Format string for PhoneId Live request uri. The phone number is
        /// filled into the format field.
        /// </summary>
        private const string LiveResourceFormatString = "/v1/phoneid/live/{0}";
        private const string V1_PHONEID_NUMBER_DEACTIVATION = "/v1/phoneid/number_deactivation/{0}";
        private const string PHONEID_RESOURCE = "/v1/phoneid/";

        /// <summary>
        /// Initializes a new instance of the RawPhoneIdService class with a supplied credential and uri.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        public RawPhoneIdService()
            : this("default")
        {
        }

        /// <summary>
        /// Initializes a new instance of the RawPhoneIdService class with a supplied credential and uri.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        public RawPhoneIdService(string accountName)
            : base(null, null, accountName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RawPhoneIdService class with a supplied credential and uri.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        public RawPhoneIdService(TeleSignServiceConfiguration configuration)
            : this(configuration, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RawPhoneIdService class with a supplied credential and uri and
        /// a web requester. In general you do not need to use this constructor unless you want to intercept
        /// the web requests for logging/debugging/testing purposes.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        /// <param name="webRequester">The web requester to use.</param>
        public RawPhoneIdService(
                    TeleSignServiceConfiguration configuration, 
                    IWebRequester webRequester)
            : base(configuration, webRequester)
        {
        }

        /// <summary>
        /// Performs a TeleSign PhoneID Standard lookup. This product provides information
        /// about the location the phone was registered, the type of phone (mobile, fixed,
        /// VOIP, etc).
        /// </summary>
        /// <param name="phoneNumber">The phone number to lookup.</param>
        /// <param name="standardParams">The phone number to lookup.</param>
        /// <returns>The raw JSON string response.</returns>
        public TeleSignResponse StandardLookupRaw(
                    string phoneNumber,
                    Dictionary<String, String> standardParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            string resourceName = string.Format(
                        CultureInfo.InvariantCulture,
                        RawPhoneIdService.StandardResourceFormatString, 
                        phoneNumber);
            
            WebRequest request = this.ConstructWebRequest(
                        resourceName,
                        "GET",
                        standardParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        /// <summary>
        /// Performs a TeleSign PhoneID Contact lookup. This product provides all the
        /// information that is provided by PhoneID Standard as well as additional
        /// information about the owner of the phone such as name and address.
        /// </summary>
        /// <param name="phoneNumber">The phone number to lookup.</param>
        /// <param name="useCaseId">The use case for the lookup. (Restricted set of values).</param>
        /// <param name="contactParams"></param>
        /// <returns>The raw JSON string response.</returns>
        public TeleSignResponse ContactLookupRaw(
                    string phoneNumber, 
                    string useCaseId = RawPhoneIdService.DefaultUseCaseId, Dictionary<String, String> contactParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            string resourceName = string.Format(
                        CultureInfo.InvariantCulture,
                        RawPhoneIdService.ContactResourceFormatString, 
                        phoneNumber);
            if (null == contactParams)
                contactParams = new Dictionary<string, string>();

            contactParams.Add("ucid", useCaseId);

            WebRequest request = this.ConstructWebRequest(
                        resourceName,
                        "GET",
                        contactParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        /// <summary>
        /// Performs a TeleSign PhoneID Score lookup. This product provides all the
        /// information that is provided by PhoneID standard as well as a risk
        /// value.
        /// </summary>
        /// <param name="phoneNumber">The phone number to lookup.</param>
        /// <param name="useCaseId">The use case for the lookup. (Restricted set of values).</param>
        /// <param name="scoreParams"></param>
        /// <returns>The raw JSON string response.</returns>
        public TeleSignResponse ScoreLookupRaw(
                    string phoneNumber,
                    string useCaseId = RawPhoneIdService.DefaultUseCaseId,
                     Dictionary<String, String> scoreParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            string resourceName = string.Format(
                        CultureInfo.InvariantCulture,
                        RawPhoneIdService.ScoreResourceFormatString,
                        phoneNumber);
            if (null == scoreParams)
                scoreParams = new Dictionary<string, string>();

            scoreParams.Add("ucid", useCaseId);

            WebRequest request = this.ConstructWebRequest(
                        resourceName,
                        "GET",
                        scoreParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        /// <summary>
        /// Performs a TeleSign PhoneID Live lookup. This product provides all the
        /// information that is provided by PhoneID standard as well as additional
        /// roaming and phone status information.
        /// </summary>
        /// <param name="phoneNumber">The phone number to lookup.</param>
        /// <param name="useCaseId">The use case for the lookup. (Restricted set of values).</param>
        /// <param name="liveParams"></param>
        /// <returns>The raw JSON string response.</returns>
        public TeleSignResponse LiveLookupRaw(
                    string phoneNumber,
                    string useCaseId = RawPhoneIdService.DefaultUseCaseId,
                    Dictionary<String, String> liveParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            string resourceName = string.Format(
                        CultureInfo.InvariantCulture,
                        RawPhoneIdService.LiveResourceFormatString,
                        phoneNumber);

            if (null == liveParams)
                liveParams = new Dictionary<string, string>();

            liveParams.Add("ucid", useCaseId);

            WebRequest request = this.ConstructWebRequest(
                        resourceName,
                        "GET",
                        liveParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }
        
        /// <summary>
        /// The PhoneID Number Deactivation API determines whether a phone number has been deactivated and when, 
        /// based on carriers' phone number data and TeleSign's proprietary analysis. See 
        /// https://developer.telesign.com/docs/rest_api-phoneid-number-deactivation for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="useCaseId"></param>
        /// <param name="deactivationParams"></param>
        /// <returns></returns>
        public TeleSignResponse NumberDeactivationRaw(string phoneNumber,
            string useCaseId = RawPhoneIdService.DefaultUseCaseId,
                Dictionary<String, String> deactivationParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            string resourceName = string.Format(
                        CultureInfo.InvariantCulture,
                        RawPhoneIdService.V1_PHONEID_NUMBER_DEACTIVATION,
                        phoneNumber);

            if (null == deactivationParams)
                deactivationParams = new Dictionary<string, string>();

            deactivationParams.Add("ucid", useCaseId);

            WebRequest request = this.ConstructWebRequest(
                        resourceName,
                        "GET",
                        deactivationParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        /// <summary>
        /// The PhoneID API provides a cleansed phone number, phone type, and telecom carrier information to determine the best communication method - SMS or voice. See https://developer.telesign.com/docs/phoneid-api for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="phoneidParams"></param>
        /// <returns></returns>
        public TeleSignResponse PhoneIdRaw(string phoneNumber, Dictionary<String, String> phoneidParams = null) {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            string resourceName = string.Format(
                        CultureInfo.InvariantCulture,
                        RawPhoneIdService.PHONEID_RESOURCE,
                        phoneNumber);

            WebRequest request = this.ConstructWebRequest(resourceName, "POST", phoneidParams, AuthenticationMethod.HmacSha256);

            return this.WebRequester.ReadTeleSignResponse(request);
        }
    }
}
