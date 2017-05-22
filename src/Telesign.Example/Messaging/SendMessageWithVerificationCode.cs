using System;

namespace Telesign.Example.Messaging
{
    class SendMessageWithVerificationCode
    {
        static void Main(string[] args)
        {
            string customerId = "FFFFFFFF-EEEE-DDDD-1234-AB1234567890";
            string apiKey = "EXAMPLETE8sTgg45yusumoN6BYsBVkh+yRJ5czgsnCehZaOYldPJdmFh6NeX8kunZ2zU1YWaUw/0wV6xfw==";

            string phoneNumber = "phone_number";

            string verifyCode = "12345";
            string message = string.Format("Your code is {0}", verifyCode);
            string messageType = "OTP";

            try
            {
                MessagingClient messagingClient = new MessagingClient(customerId, apiKey);
                RestClient.TelesignResponse telesignResponse = messagingClient.Message(phoneNumber, message, messageType);
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