using System;
using System.Collections.Generic;

namespace TeleSign.Services.Telebureau
{
    public class TelebureauClient: RawTelebureauService
    {
        public TelebureauClient(TeleSignServiceConfiguration configuration): base(configuration) { }
        /// <summary>
        /// Creates a telebureau event corresponding to supplied data.
        /// See https://developer.telesign.com/docs/rest_api-telebureau for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="fraud_type"></param>
        /// <param name="occurred_at"></param>
        /// <param name="createEventParams"></param>
        /// <returns></returns>
        public TeleSignResponse Create(string phoneNumber, string fraud_type, string occurred_at, Dictionary<string, string> createEventParams = null) {
            TeleSignResponse response = new TeleSignResponse();
            try
            {
                response = this.CreateRaw(phoneNumber, fraud_type, occurred_at, createEventParams);
            }
            catch (Exception e)
            {
                throw new ResponseParseException(
                            "Error parsing Telebureau Create response",
                            response.ToString(),
                            e);
            }

            return response;
        }
        /// <summary>
        /// Retrieves the fraud event status. You make this call in your web application after completion of create
        /// transaction for a telebureau event.
        /// See https://developer.telesign.com/docs/rest_api-telebureau for detailed API documentation.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="retrieveParams"></param>
        /// <returns></returns>
        public TeleSignResponse Retrieve(string referenceId, Dictionary<string, string> retrieveParams = null) {
            TeleSignResponse response = new TeleSignResponse();
            try
            {
                response = this.RetrieveRaw(referenceId, retrieveParams);
            }
            catch (Exception e)
            {
                throw new ResponseParseException(
                            "Error parsing Telebureau Retrieve response",
                            response.ToString(),
                            e);
            }

            return response;
        }
        /// <summary>
        /// Deletes a previously submitted fraud event. You make this call in your web application after completion of the
        /// create transaction for a telebureau event.
        /// See https://developer.telesign.com/docs/rest_api-telebureau for detailed API documentation.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="deleteParams"></param>
        /// <returns></returns>
        public TeleSignResponse Delete(string referenceId, Dictionary<string, string> deleteParams = null)
        {
            TeleSignResponse response = new TeleSignResponse();
            try
            {
                response = this.DeleteRaw(referenceId, deleteParams);
            }
            catch (Exception e)
            {
                throw new ResponseParseException(
                            "Error parsing Telebureau Delete response",
                            response.ToString(),
                            e);
            }

            return response;
        }
    }
}
