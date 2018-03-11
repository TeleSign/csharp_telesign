using System;
using System.Collections.Generic;

namespace Telesign.Example.PhoneId
{
    class Addons
    {
        static void Main(string[] args)
        {
            string customerId = "FFFFFFFF-EEEE-DDDD-1234-AB1234567890";
            string apiKey = "EXAMPLETE8sTgg45yusumoN6BYsBVkh+yRJ5czgsnCehZaOYldPJdmFh6NeX8kunZ2zU1YWaUw/0wV6xfw==";

            string phoneNumber = "phone_number";

            Dictionary<string, object> parameters = {
                "addons": [
                    "contact"
                ],
            };
            try
            {
                PhoneIdClient phoneIdClient = new PhoneIdClient(customerId, apiKey);
                RestClient.TelesignResponse telesignResponse = phoneIdClient.PhoneId(phoneNumber, parameters);

                if (telesignResponse.OK)
                {
                    Console.WriteLine(telesignResponse.Json);
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