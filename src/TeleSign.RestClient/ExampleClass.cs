using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Security;
using static TeleSign.RestClient.TeleSignRestClient;

namespace TeleSign.RestClient
{
    class ExampleClass
    {
        static string customerId = "TeleSign provided customer ID";
        static string apiKey = "TeleSign provided apiKey";
        static string restEndpoint = "https://rest-api.telesign.com";
        //static string restEndpoint = null;

        static void Main() {
            string phoneNo = "Phone no to use";
            MessagingExample(phoneNo);
        }

        /// <summary>
        /// Method to Set proxy
        /// </summary>
        /// <returns></returns>
        private static WebProxy SetProxy()
        {            
            WebProxy tsProxy = null;
            string httpProxyIPAddress = "Enter the proxy IPAddress";
            int httpProxyPort = 8080;// Enter the proxy port
            // create web proxy
            tsProxy = new WebProxy(httpProxyIPAddress, httpProxyPort);
            // provide proxy credentials if needed
            string httpProxyUsername = "Enter username for proxy if credential needed";
            string httpProxyPassword = "Enter proxy password";            
            string domain = "Enter the domain name to be used";
            tsProxy.Credentials = new NetworkCredential(httpProxyUsername, httpProxyPassword, domain);

            return tsProxy;
        }

        public static void MessagingExample(string phoneNo)
        {
            WebProxy tsProxy = SetProxy();
            // 1. Initialize MessagingClient
            MessagingClient messagingClient = new MessagingClient(customerId, apiKey, restEndpoint, timeout: 5000, readWriteTimeout: 5000, proxy: tsProxy);
            // 2. Messaging Api parameters
            string phone_number = phoneNo;
            string message = "You're scheduled for a dentist appointment at 2:30PM.";
            string messageType = "ARN";
            // 3. Make Messaging API call get TeleSign Response
            TeleSignResponse msgResponse = messagingClient.Message(phone_number, message, messageType);
            // 4. From the message response JSON fetch reference_id
            JObject teleSignJsonObject = msgResponse.Json;
            string reference_id = teleSignJsonObject["reference_id"].ToString();
            // 5. using reference_id from 4 get status body
            TeleSignResponse msgStatus = messagingClient.Status(reference_id);
            Console.WriteLine(string.Format("Message status is {0}", msgStatus.Body));
        }
    }
}
