using System;

namespace Telesign.Example.Voice
{
    class SendVoiceCallWithVerificationCode
    {
        static void Main(string[] args)
        {
            string customerId = "FFFFFFFF-EEEE-DDDD-1234-AB1234567890";
            string apiKey = "EXAMPLETE8sTgg45yusumoN6BYsBVkh+yRJ5czgsnCehZaOYldPJdmFh6NeX8kunZ2zU1YWaUw/0wV6xfw==";

            string phoneNumber = "phone_number";

            string verifyCode = "12345";
            string message = string.Format("Hello, your code is {0}. Once again, your code is {1}. Goodbye.", verifyCode, verifyCode);
            string messageType = "OTP";

            try
            {
                VoiceClient voiceClient = new VoiceClient(customerId, apiKey);
                RestClient.TelesignResponse telesignResponse = voiceClient.Call(phoneNumber, message, messageType);
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