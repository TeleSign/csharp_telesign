using System.Collections.Generic;
using System.Net;
namespace TeleSign.Services.Telebureau
{
    public class TelebureauClient : TeleSignService
    {
        private const string TELEBUREAU_CREATE_RESOURCE = "/v1/telebureau/event";
        private const string TELEBUREAU_RETRIEVE_RESOURCE = "/v1/telebureau/event/{0}";
        private const string TELEBUREAU_DELETE_RESOURCE = "/v1/telebureau/event/{0}";

        public TelebureauClient(TeleSignServiceConfiguration configuration) :base(configuration, null) { }

        /// <summary>
        /// Creates a telebureau event corresponding to supplied data.
        /// See https://developer.telesign.com/docs/rest_api-telebureau for detailed API documentation.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="fraud_type"></param>
        /// <param name="occurred_at"></param>
        /// <param name="createEventParams"></param>
        /// <returns></returns>
        public TeleSignResponse Create(string phoneNumber, string fraud_type, string occurred_at, Dictionary<string, string> createEventParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            if (null == createEventParams)
                createEventParams = new Dictionary<string, string>();

            createEventParams.Add("phone_number", phoneNumber);
            createEventParams.Add("message", fraud_type);
            createEventParams.Add("message_type", occurred_at);

            WebRequest request = this.ConstructWebRequest(TELEBUREAU_CREATE_RESOURCE, "POST", createEventParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        /// <summary>
        /// Retrieves the fraud event status. You make this call in your web application after completion of create
        /// transaction for a telebureau event.
        /// See https://developer.telesign.com/docs/rest_api-telebureau for detailed API documentation.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="retrieveParams"></param>
        /// <returns></returns>
        public TeleSignResponse Retrieve(string referenceId, Dictionary<string, string> retrieveParams = null)
        {
            CheckArgument.NotNullOrEmpty(referenceId, "reference_id");
            if (null == retrieveParams)
                retrieveParams = new Dictionary<string, string>();

            retrieveParams.Add("reference_id", referenceId);

            string resourceName = string.Format(TELEBUREAU_RETRIEVE_RESOURCE, referenceId);

            WebRequest request = this.ConstructWebRequest(resourceName, "GET", retrieveParams);

            return this.WebRequester.ReadTeleSignResponse(request);
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
            CheckArgument.NotNullOrEmpty(referenceId, "reference_id");
            if (null == deleteParams)
                deleteParams = new Dictionary<string, string>();

            deleteParams.Add("reference_id", referenceId);

            string resourceName = string.Format(TELEBUREAU_DELETE_RESOURCE, referenceId);

            WebRequest request = this.ConstructWebRequest(resourceName, "DELETE", deleteParams);

            return this.WebRequester.ReadTeleSignResponse(request);
        }
    }
}
