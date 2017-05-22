using System;

namespace Telesign.Example.PhoneId
{
    class Cleansing
    {
        static void Main(string[] args)
        {
            string customerId = "FFFFFFFF-EEEE-DDDD-1234-AB1234567890";
            string apiKey = "EXAMPLETE8sTgg45yusumoN6BYsBVkh+yRJ5czgsnCehZaOYldPJdmFh6NeX8kunZ2zU1YWaUw/0wV6xfw==";

            string phoneNumber = "phone_number";

            string extraDigit = "0";
            string incorrectPhoneNumber = string.Format("{0}{1}", phoneNumber, extraDigit);

            try
            {
                PhoneIdClient phoneIdClient = new PhoneIdClient(customerId, apiKey);
                RestClient.TelesignResponse telesignResponse = phoneIdClient.PhoneId(incorrectPhoneNumber);

                if (telesignResponse.OK)
                {
                    Console.WriteLine(string.Format("Cleansed phone number has country code {0} and phone number is {0}.",
                        telesignResponse.Json["numbering"]["cleansing"]["call"]["country_code"],
                        telesignResponse.Json["numbering"]["cleansing"]["call"]["phone_number"]));

                    Console.WriteLine(string.Format("Original phone number was {0}.",
                            telesignResponse.Json["numbering"]["original"]["complete_phone_number"]));
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