//-----------------------------------------------------------------------
// <copyright file="PhoneIdService.cs" company="TeleSign Corporation">
//     Copyright (c) TeleSign Corporation 2012.
// </copyright>
// <license>MIT</license>
// <author>Zentaro Kavanagh</author>
//-----------------------------------------------------------------------

namespace TeleSign.Services.PhoneId
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The TeleSign PhoneID service. This provides 3 services. PhoneID Contact, 
    /// PhoneID Standard and PhoneID Score. TODO: Link to other documentation or more detail here?
    /// </summary>
    public class PhoneIdService : RawPhoneIdService
    {
        /// <summary>
        /// The parser that transforms the raw JSON responses
        /// </summary>
        private IPhoneIdResponseParser responseParser;

        /// <summary>
        /// Initializes a new instance of the PhoneIdService class with
        /// configuration.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        public PhoneIdService(TeleSignServiceConfiguration configuration)
            : this(configuration, null, new JsonDotNetPhoneIdResponseParser())
        {
        }

        /// <summary>
        /// Initializes a new instance of the PhoneIdService class with
        /// a configuration and a custom web requester and response parser. 
        /// You generally don't need to use this constructor.
        /// </summary>
        /// <param name="configuration">The configuration information for the service.</param>
        /// <param name="webRequester">The web requester to use.</param>
        /// <param name="responseParser">The response parser to use.</param>
        public PhoneIdService(
                    TeleSignServiceConfiguration configuration, 
                    IWebRequester webRequester,
                    IPhoneIdResponseParser responseParser)
            : base(configuration, webRequester)
        {
            // TODO: null check and possible ifdef for JSON.Net
            this.responseParser = responseParser;
        }

        /// <summary>
        /// Performs a PhoneId Standard lookup on a phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number to lookup.</param>
        /// <param name="standardParams"></param>
        /// <returns>
        /// A StandardPhoneIdResponse object containing both status of the transaction
        /// and the resulting data (if successful).
        /// </returns>
        public TSResponse StandardLookup(string phoneNumber,
            Dictionary<String, String> standardParams = null)
        {
            CheckArgument.NotNullOrEmpty(phoneNumber, "phoneNumber");

            TSResponse response = new TSResponse();            

            try
            {
                response = this.StandardLookupRaw(phoneNumber, standardParams);
                return response;
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing PhoneID Standard response",
                            response.ToString(),
                            x);
            }
        }

        /// <summary>
        /// Performs a PhoneID Contact lookup on a phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number to lookup.</param>
        /// <param name="ucid"></param>
        /// <param name="contactParams"></param>
        /// <returns>
        /// A ContactPhoneIdResponse object containing both status of the transaction
        /// and the resulting data (if successful).
        /// </returns>
        public TSResponse ContactLookup(string phoneNumber, string ucid,
            Dictionary<String, String> contactParams = null)
        {
            CheckArgument.NotNullOrEmpty(phoneNumber, "phoneNumber");

            TSResponse response = new TSResponse();
            
            try
            {
                response = this.ContactLookupRaw(phoneNumber, ucid, contactParams);
                return response;
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing PhoneID Contact response",
                            response.ToString(),
                            x);
            }
        }

        /// <summary>
        /// Performs a PhoneID Score lookup on a phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number to lookup.</param>
        /// <param name="ucid"></param>
        /// <param name="scoreParams"></param>
        /// <returns>
        /// A ScorePhoneIdResponse object containing both status of the transaction
        /// and the resulting data (if successful).
        /// </returns>
        public TSResponse ScoreLookup(string phoneNumber, string ucid,
            Dictionary<String, String> scoreParams = null)
        {
            CheckArgument.NotNullOrEmpty(phoneNumber, "phoneNumber");
            TSResponse response = new TSResponse();           

            try
            {
                response = this.ScoreLookupRaw(phoneNumber, ucid, scoreParams);
                return response;
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing Phone Id Score",
                            response.ToString(),
                            x);
            }
        }

        /// <summary>
        /// Performs a PhoneID Live lookup on a phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number to lookup.</param>
        /// <param name="ucid"></param>
        /// <param name="liveParams"></param>
        /// <returns>
        /// A PhoneIdLiveResponse object containing both status of the transaction
        /// and the resulting data (if successful).
        /// </returns>
        public TSResponse LiveLookup(string phoneNumber, string ucid,
            Dictionary<String, String> liveParams = null)
        {
            CheckArgument.NotNullOrEmpty(phoneNumber, "phoneNumber");
            TSResponse response = new TSResponse();           

            try
            {
                response = this.LiveLookupRaw(phoneNumber, ucid, liveParams);
                return response;
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing Phone Id Live",
                            response.ToString(),
                            x);
            }
        }

        /// <summary>
        /// The PhoneID Number Deactivation API determines whether a phone number has been deactivated and when, 
        /// based on carriers' phone number data and TeleSign's proprietary analysis. See 
        /// https://developer.telesign.com/docs/rest_api-phoneid-number-deactivation for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="ucid"></param>
        /// <param name="deactivationParams"></param>
        /// <returns></returns>
        public TSResponse NumberDeactivation(string phoneNumber, string ucid,
                Dictionary<String, String> deactivationParams = null)
        {
            CheckArgument.NotNullOrEmpty(phoneNumber, "phoneNumber");
            TSResponse response = new TSResponse();

            try
            {
                response = this.NumberDeactivationRaw(phoneNumber, ucid, deactivationParams);
                return response;
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing Phone Id Number Deactivation",
                            response.ToString(),
                            x);
            }
        }

        /// <summary>
        /// The PhoneID API provides a cleansed phone number, phone type, and telecom carrier information to determine the best communication method - SMS or voice. See https://developer.telesign.com/docs/phoneid-api for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="ucid"></param>
        /// <param name="phoneidParams"></param>
        /// <returns></returns>
        public TSResponse PhoneId(string phoneNumber, String ucid, Dictionary<String, String> phoneidParams = null)
        {
            CheckArgument.NotNullOrEmpty(phoneNumber, "phoneNumber");
            TSResponse response = new TSResponse();

            try
            {
                response = this.PhoneId(phoneNumber, ucid, phoneidParams);
                return response;
            }
            catch (Exception x)
            {
                throw new ResponseParseException(
                            "Error parsing PhoneId",
                            response.ToString(),
                            x);
            }
        }
}
