using System;

namespace Telesign.Example.PhoneId
{
    class CheckPhoneTypeToBlockVoip
    {
        static void Main(string[] args)
        {
            string customerId = "FFFFFFFF-EEEE-DDDD-1234-AB1234567890";
            string apiKey = "EXAMPLETE8sTgg45yusumoN6BYsBVkh+yRJ5czgsnCehZaOYldPJdmFh6NeX8kunZ2zU1YWaUw/0wV6xfw==";

            string phoneNumber = "phone_number";

            string phoneTypeVoip = "5";

            try
            {
                PhoneIdClient phoneIdClient = new PhoneIdClient(customerId, apiKey);
                RestClient.TelesignResponse telesignResponse = phoneIdClient.PhoneId(phoneNumber);

                if (telesignResponse.OK)
                {
                    if (telesignResponse.Json["phone_type"]["code"].ToString() == phoneTypeVoip)
                    {
                        Console.WriteLine(string.Format("Phone number {0} is a VOIP phone.", phoneNumber));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Phone number {0} is not a VOIP phone.", phoneNumber));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }
    }
}