using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleSign.Services.AutoVerify
{
    public class AutoVerifyClient : RawAutoVerifyService
    {
        public AutoVerifyClient(TeleSignServiceConfiguration configuration) : base(configuration)
        {
        }

        public TSResponse Status(string externalId, Dictionary<String, String> statusParams = null)
        {
            CheckArgument.NotNullOrEmpty(externalId, "external_id");

            TSResponse response = new TSResponse();
            try
            {
                response = this.StatusRaw(externalId, statusParams);
            }
            catch (Exception e)
            {
                throw new ResponseParseException(
                            "Error parsing Message response",
                            response.ToString(),
                            e);
            }
            return response;
        }
    }
}
