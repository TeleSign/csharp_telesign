using System.Collections.Generic;
using System.Net;
namespace TeleSign.Services.Telebureau
{
    public class RawTelebureauService : TeleSignService
    {
        private const string TELEBUREAU_CREATE_RESOURCE = "/v1/telebureau/event";
        private const string TELEBUREAU_RETRIEVE_RESOURCE = "/v1/telebureau/event/{0}";
        private const string TELEBUREAU_DELETE_RESOURCE = "/v1/telebureau/event/{0}";

        public RawTelebureauService(TeleSignServiceConfiguration configuration) :base(configuration, null) { }

        public TSResponse CreateRaw(string phoneNumber, string fraud_type, string occurred_at, Dictionary<string, string> createEventParams = null)
        {
            phoneNumber = this.CleanupPhoneNumber(phoneNumber);

            if (null == createEventParams)
                createEventParams = new Dictionary<string, string>();

            createEventParams.Add("phone_number", phoneNumber);
            createEventParams.Add("message", fraud_type);
            createEventParams.Add("message_type", occurred_at);

            WebRequest request = this.ConstructWebRequest(TELEBUREAU_CREATE_RESOURCE, "POST", createEventParams, AuthenticationMethod.HmacSha256);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        public TSResponse RetrieveRaw(string referenceId, Dictionary<string, string> retrieveParams = null)
        {
            CheckArgument.NotNullOrEmpty(referenceId, "reference_id");
            if (null == retrieveParams)
                retrieveParams = new Dictionary<string, string>();

            retrieveParams.Add("reference_id", referenceId);

            string resourceName = string.Format(TELEBUREAU_RETRIEVE_RESOURCE, referenceId);

            WebRequest request = this.ConstructWebRequest(resourceName, "GET", retrieveParams, AuthenticationMethod.HmacSha256);

            return this.WebRequester.ReadTeleSignResponse(request);
        }

        public TSResponse DeleteRaw(string referenceId, Dictionary<string, string> deleteParams = null)
        {
            CheckArgument.NotNullOrEmpty(referenceId, "reference_id");
            if (null == deleteParams)
                deleteParams = new Dictionary<string, string>();

            deleteParams.Add("reference_id", referenceId);

            string resourceName = string.Format(TELEBUREAU_DELETE_RESOURCE, referenceId);

            WebRequest request = this.ConstructWebRequest(resourceName, "DELETE", deleteParams, AuthenticationMethod.HmacSha256);

            return this.WebRequester.ReadTeleSignResponse(request);
        }
    }
}
