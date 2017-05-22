using System;
using System.Collections.Generic;

namespace Telesign.Example.Voice
{
    class SendVoiceCallFrench
    {
        static void Main(string[] args)
        {
            string customerId = "FFFFFFFF-EEEE-DDDD-1234-AB1234567890";
            string apiKey = "EXAMPLETE8sTgg45yusumoN6BYsBVkh+yRJ5czgsnCehZaOYldPJdmFh6NeX8kunZ2zU1YWaUw/0wV6xfw==";

            string phoneNumber = "phone_number";

            string message = "N'oubliez pas d'appeler votre mère pour son anniversaire demain.";
            string messageType = "ARN";

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"voice", "f-FR-fr" }
            };

            try
            {
                VoiceClient voiceClient = new VoiceClient(customerId, apiKey);
                RestClient.TelesignResponse telesignResponse = voiceClient.Call(phoneNumber, message, messageType, parameters);
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