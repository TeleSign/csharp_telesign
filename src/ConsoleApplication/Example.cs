using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSign.Services;
using TeleSign.Services.Voice;
using TeleSign.Services.AutoVerify;
using TeleSign.Services.Messaging;
using Newtonsoft.Json.Linq;
using TeleSign.Services.PhoneId;
using TeleSign.Services.Score;
using TeleSign.Services.Verify;

namespace ConsoleApplication
{
    class Example
    {
        public static TeleSignServiceConfiguration GetConfiguration()
        {
            // return Null below to pick config from TeleSign.config.xml.
            // 
            // If you comment out return null, and uncomment below block,
            // then by filling in your customer id and secret key
            // you can construct the configuration object in code.
            return null;

            ////Guid customerId = Guid.Parse("*** Customer ID goes here ***");
            ////string secretKey = "*** Secret Key goes here ***";
            ////
            ////TeleSignCredential credential = new TeleSignCredential(
            ////            customerId,
            ////            secretKey);
            ////
            ////return new TeleSignServiceConfiguration(credential);
        }
        static void Main()
        {
            string phoneNo = "918427434777";
            string externalId = "external_id";

            SendCallWithVerificationCode(phoneNo);
            GetStatusByExternalId(externalId);
            MessagingExample(phoneNo);
            Cleansing(phoneNo);
            CheckPhoneNoRiskLevel(phoneNo);
            CheckPhoneTypeToBlockVoip(phoneNo);
            CheckPhoneNumberDeactivated(phoneNo);
            CheckPhoneNumberRiskLevel(phoneNo);
            PhoneIdStandard(phoneNo);
            MessagingExampleWithCode(phoneNo);
            SendCustomSms(phoneNo);
            SendCustomSmsInDifferentLanguage(phoneNo);
            SendCustomSmsWithCustomSenderId(phoneNo);
            SendSmsWithVerificationCode(phoneNo);
            SendVoiceCallWithVerificationCode(phoneNo);
            SendCustomVoiceCallWithTextToSpeech(phoneNo);
            SendCustomVoiceCallInDifferentLanguage(phoneNo);
            SendVoiceCall(phoneNo);
            SendVoiceCallFrench(phoneNo);
        }

        public static void CheckPhoneNoRiskLevel(string phoneNo)
        {
            // 1. Initialize ScoreClient
            ScoreClient scoreClient = new ScoreClient(GetConfiguration());
            // 2. Score Api parameters
            string phone_number = phoneNo;
            string account_lifecycle_event = "create";

            // 3. Fetch the response
            TeleSignResponse response = scoreClient.Score(phone_number, account_lifecycle_event);

            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                JObject jsonObject = response.Json;
                String riskLevel = jsonObject["risk"]["level"].ToString();
                String recommendation = jsonObject["risk"]["recommendation"].ToString();
                Console.WriteLine("Phone number {0} has a '{1}' risk level and the recommendation is to '{2}' the transaction.", phone_number, riskLevel, recommendation);
            }
        }

        public static void Cleansing(string phoneNo)
        {
            // 1. Initialize PhoneIdService
            PhoneIdClient phoneId = new PhoneIdClient(GetConfiguration());
            // 2. PhoneID Api parameters
            string phone_number = phoneNo;
            string extra_digit = "0";
            string incorrect_phone_number = phone_number + extra_digit;
            Dictionary<string, string> phoneIdParams = new Dictionary<string, string>();
            phoneIdParams.Add("account_lifecycle_event", "create");
            // 3. Fetch the response
            TeleSignResponse response = phoneId.PhoneId(incorrect_phone_number, phoneIdParams);
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                JObject jsonObject = response.Json;

                JToken numbering = jsonObject["numbering"];
                JToken cleansing = numbering["cleansing"];
                JToken call = cleansing["call"];

                String country_code = call["country_code"].ToString();
                String cleansedNo = call["phone_number"].ToString();

                Console.WriteLine("Cleansed phone number has country code {0} and phone number is {1}.", country_code, cleansedNo);
                Console.WriteLine("Original phone number was {0}", numbering["original"]["complete_phone_number"].ToString());
            }
        }

        public static void MessagingExample(string phoneNo)
        {            
            // 1. Initialize MessagingClient
            MessagingClient messagingClient = new MessagingClient(GetConfiguration());
            // 2. Messaging Api parameters
            string phone_number = phoneNo;
            string message = "You're scheduled for a dentist appointment at 2:30PM.";
            string messageType = "ARN";
            // 3. Make Messaging API call get TeleSign Response
            TeleSignResponse response = messagingClient.Message(phone_number, message, messageType);
            // 4. Read TeleSign Response body.
            if (((int)response.StatusCode) >= 200 && ((int)response.StatusCode) < 300)
            {
                JObject teleSignJsonObject = response.Json;
                string reference_id = teleSignJsonObject["reference_id"].ToString();
                // 1.1 Get the reference ID/
                Console.WriteLine(reference_id);
            }
        }

        public static void GetStatusByExternalId(string externalId)
        {   
            // 1. Initialize AutoVerifyClient
            AutoVerifyClient autoVerifyClient = new AutoVerifyClient(GetConfiguration());
            // 2. AutoVerify Api parameters            
            string external_id = externalId;
            // 3. Make Messaging API call get TeleSign Response
            TeleSignResponse response = autoVerifyClient.Status(external_id);
            // 4. Read TeleSign Response body.            
            if (((int)response.StatusCode) >= 200 && ((int)response.StatusCode) < 300)
            {
                JObject teleSignJsonObject = response.Json;
                JToken statusObject = teleSignJsonObject["status"];
                Console.WriteLine("AutoVerify transaction with external_id as {0} has status code  as {1} and status description as {2}.", external_id, statusObject["code"].ToString(), statusObject["description"].ToString());
            }
        }

        public static void SendCallWithVerificationCode(string phoneNo)
        {   
            // 1. Initialize VoiceClient
            VoiceClient voiceClient = new VoiceClient(GetConfiguration());
            // 2. VerifyService Api parameters
            string phone_number = phoneNo;

            // 2.1 Generating 5 digit random no to send
            Random r = new Random();
            string verifyCode = r.Next(10000, 99999).ToString();
            // formatting code to include commas
            string formattedcode = string.Join(",", verifyCode.ToCharArray());
            string message = "Hello, your code is " + formattedcode + ". Once again, your code is "
                + formattedcode + ". Goodbye.";
            string message_type = "OTP";

            // 3. Make Voice call and get TeleSign Response
            TeleSignResponse response = voiceClient.Call(phone_number, message, message_type);
            // 4. Read TeleSign Response body.
            if (response.StatusLine.Equals("created", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Call sent with message");
                Console.WriteLine("Please enter the verification code you were sent: ");
                string user_entered_verify_code = Console.ReadLine();
                if (user_entered_verify_code.Equals(verifyCode))
                    Console.WriteLine("Your code is correct.");
                else
                    Console.WriteLine("Your code is not correct.");
            }
        }
        
        public static void CheckPhoneTypeToBlockVoip(string phoneNo)
        {            

            // 1. Initialize PhoneIdService
            PhoneIdService phoneId = new PhoneIdService(GetConfiguration());
            // 2. PhoneID Api parameters
            string phone_number = phoneNo;
            string phone_type_voip = "5";
            // 3. Fetch the response
            TeleSignResponse response = phoneId.StandardLookup(phone_number);
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                JObject jsonObject = response.Json;
                JToken phone_type = jsonObject["phone_type"];
                if (phone_type["code"].ToString().Equals(phone_type_voip))
                    Console.WriteLine("Phone number {0} is a VOIP phone.", phone_number);
                else
                    Console.WriteLine("Phone number {0} is a NOT a VOIP phone.", phone_number);
            }
        }

        public static void CheckPhoneNumberDeactivated(string phoneNo)
        {
            // 1. Initialize PhoneIdService
            PhoneIdService phoneId = new PhoneIdService(GetConfiguration());
            // 2. PhoneID Api parameters
            string phone_number = phoneNo;
            string ucid = "ATCK";
            // 3. Fetch the response
            TeleSignResponse response = phoneId.NumberDeactivation(phone_number, ucid);
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                JObject jsonObject = response.Json;

                JToken number_deactivation = jsonObject["number_deactivation"];
                if (null != number_deactivation["last_deactivated"] && String.Empty != number_deactivation["last_deactivated"].ToString() && (Boolean)number_deactivation["last_deactivated"])
                {
                    string number = number_deactivation["number"].ToString();
                    string last_deactivated = number_deactivation["last_deactivated"].ToString();
                    Console.WriteLine("Phone number {0} was last deactivated {1}.", number, last_deactivated);
                }
                else
                {
                    string number = number_deactivation["number"].ToString();
                    Console.WriteLine("Phone number {0} has not been deactivated.", number);
                }
            }
        }

        
        public static void CheckPhoneNumberRiskLevel(string phoneNo)
        {           

            // 1. Initialize PhoneIdService
            PhoneIdService phoneId = new PhoneIdService(GetConfiguration());
            // 2. PhoneID Api parameters
            string phone_number = phoneNo;
            string ucid = "BACF";
            // 3. Fetch the response
            TeleSignResponse response = phoneId.ScoreLookup(phone_number, ucid);
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                JObject jsonObject = response.Json;

                JToken riskJsonObject = jsonObject["risk"];

                String level = riskJsonObject["level"].ToString();
                String recommendation = riskJsonObject["recommendation"].ToString();
                Console.WriteLine("Phone number {0} has a '{1}' risk level and the recommendation is to '{2}' the transaction.",
                        phone_number, level, recommendation);
            }
        }
        
        public static void PhoneIdStandard(string phoneNo)
        {
            
            string phoneNumber = phoneNo;

            PhoneIdService service = new PhoneIdService(GetConfiguration());
            TeleSignResponse response = service.StandardLookup(phoneNumber);
        }
        
        public static void MessagingExampleWithCode(string phoneNo)
        {
            
            // 1. Initialize MessagingClient
            MessagingClient messagingClient = new MessagingClient(GetConfiguration());
            // 2. Messaging Api parameters
            string phone_number = phoneNo;
            string message = "Your code is ";
            string messageType = "OTP";
            // 2.1 Generating 5 digit random no to send
            Random r = new Random();
            string verifyCode = r.Next(10000, 99999).ToString();
            // 3. Make Messaging API call get TeleSign Response
            TeleSignResponse response = messagingClient.Message(phone_number, message + verifyCode, messageType);
            // 4. Read TeleSign Response body.
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                Console.WriteLine("Please enter the verification code you were sent: ");
                string user_entered_verify_code = Console.ReadLine();
                if (user_entered_verify_code.Equals(verifyCode))
                    Console.WriteLine("Your code is correct.");
                else
                    Console.WriteLine("Your code is not correct.");
            }
        }

        public static void SendCustomSms(string phoneNo)
        {
            
            // 1. Initialize VerifyService
            VerifyService ver = new VerifyService(GetConfiguration());
            // 2. Verify SMS Api parameters
            string phone_number = phoneNo;
            string template = "Your Widgets 'n' More verification code is $$CODE$$.";

            Dictionary<string, string> smsParams = new Dictionary<String, String>();
            smsParams.Add("template", template);

            // 3. Fetch the response
            TeleSignResponse response = ver.SendSms(phone_number, smsParams);
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                Console.WriteLine("Custom Sms with code sent");
            }
        }

        
        public static void SendCustomSmsInDifferentLanguage(string phoneNo)
        {
            
            // 1. Initialize VerifyService
            VerifyService ver = new VerifyService(GetConfiguration());
            // 2. Verify SMS Api parameters
            string phone_number = phoneNo;
            string template = "Votre code de vérification Widgets 'n' More est $$CODE$$.";

            Dictionary<string, string> smsParams = new Dictionary<String, String>();
            smsParams.Add("template", template);

            // 3. Fetch the response
            TeleSignResponse response = ver.SendSms(phone_number, smsParams);
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                Console.WriteLine("Custom Sms with code sent");
            }
        }

        
        public static void SendCustomSmsWithCustomSenderId(string phoneNo)
        {
            
            // 1. Initialize VerifyService
            VerifyService ver = new VerifyService(GetConfiguration());
            // 2. Verify SMS Api parameters
            string phone_number = phoneNo;
            // Client Services must white list any custom sender_id for it to take effect
            string my_sender_id = "my_sender_id";

            Dictionary<string, string> smsParams = new Dictionary<String, String>();
            smsParams.Add("my_sender_id", my_sender_id);

            // 3. Fetch the response
            TeleSignResponse response = ver.SendSms(phone_number, smsParams);
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                Console.WriteLine("Custom Sms with sender id sent");
            }
        }

        
        public static void SendSmsWithVerificationCode(string phoneNo)
        {
            
            // 1. Initialize VerifyService
            VerifyService ver = new VerifyService(GetConfiguration());
            // 2. VerifyService Api parameters
            string phone_number = phoneNo;
            Dictionary<string, string> smsParams = new Dictionary<String, String>();
            // 2.1 Generating 5 digit random no to send
            Random r = new Random();
            string verifyCode = r.Next(10000, 99999).ToString();
            smsParams.Add("verify_code", verifyCode);
            // 3. Make Messaging API call get TeleSign Response
            TeleSignResponse response = ver.SendSms(phone_number, smsParams);
            // 4. Read TeleSign Response body.
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                Console.WriteLine("Please enter the verification code you were sent: ");
                string user_entered_verify_code = Console.ReadLine();
                if (user_entered_verify_code.Equals(verifyCode))
                    Console.WriteLine("Your code is correct.");
                else
                    Console.WriteLine("Your code is not correct.");
            }
        }

        
        public static void SendVoiceCallWithVerificationCode(string phoneNo)
        {
            
            // 1. Initialize VerifyService
            VerifyService ver = new VerifyService(GetConfiguration());
            // 2. VerifyService Api parameters
            string phone_number = phoneNo;
            Dictionary<string, string> callParams = new Dictionary<String, String>();
            // 2.1 Generating 5 digit random no to send
            Random r = new Random();
            string verifyCode = r.Next(10000, 99999).ToString();
            callParams.Add("verify_code", verifyCode);
            // 3. Make Voice call and get TeleSign Response
            TeleSignResponse response = ver.Voice(phone_number, callParams);
            // 4. Read TeleSign Response body.
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                Console.WriteLine("Please enter the verification code you were sent: ");
                string user_entered_verify_code = Console.ReadLine();
                if (user_entered_verify_code.Equals(verifyCode))
                    Console.WriteLine("Your code is correct.");
                else
                    Console.WriteLine("Your code is not correct.");
            }
        }

        
        public static void SendCustomVoiceCallWithTextToSpeech(string phoneNo)
        {
            
            // 1. Initialize VerifyService
            VerifyService ver = new VerifyService(GetConfiguration());
            // 2. VerifyService Api parameters
            string phone_number = phoneNo;
            Dictionary<string, string> callParams = new Dictionary<String, String>();
            // 2.1 Generating 5 digit random no to send
            Random r = new Random();
            string verifyCode = r.Next(10000, 99999).ToString();
            string tts_message = "Hello, your code is " + verifyCode + ". Once again, your code is "
                + verifyCode + ". Goodbye.";

            callParams.Add("tts_message", tts_message);

            // 3. Make Voice call and get TeleSign Response
            TeleSignResponse response = ver.Voice(phone_number, callParams);
            // 4. Read TeleSign Response body.
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                Console.WriteLine("Call sent with message");
                Console.WriteLine("Please enter the verification code you were sent: ");
                string user_entered_verify_code = Console.ReadLine();
                if (user_entered_verify_code.Equals(verifyCode))
                    Console.WriteLine("Your code is correct.");
                else
                    Console.WriteLine("Your code is not correct.");
            }
        }

        
        public static void SendCustomVoiceCallInDifferentLanguage(string phoneNo)
        {
            
            // 1. Initialize VerifyService
            VerifyService ver = new VerifyService(GetConfiguration());
            // 2. VerifyService Api parameters
            string phone_number = phoneNo;
            Dictionary<string, string> callParams = new Dictionary<String, String>();
            // 2.1 Specifying a language parameter
            string language = "fr-FR";
            string tts_message = "Votre code de vérification Widgets 'n' More est $$CODE$$.";

            callParams.Add("tts_message", tts_message);
            callParams.Add("language", language);

            // 3. Make Voice call and get TeleSign Response
            TeleSignResponse response = ver.Voice(phone_number, callParams);

            // 4. Read TeleSign Response body.
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                Console.WriteLine("Call sent with message");
            }
        }

        public static void SendVoiceCall(string phoneNo)
        {
            
            // 1. Initialize VoiceClient
            VoiceClient voiceClient = new VoiceClient(GetConfiguration());
            // 2. VerifyService Api parameters
            string phone_number = phoneNo;
            string message = "You're scheduled for a dentist appointment at 2:30PM.";
            string message_type = "ARN";

            // 3. Make Voice call and get TeleSign Response
            TeleSignResponse response = voiceClient.Call(phone_number, message, message_type);
            // 4. Read TeleSign Response body.
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                Console.WriteLine("Call sent with message");
            }
        }

        
        public static void SendVoiceCallFrench(string phoneNo)
        {
            
            // 1. Initialize VoiceClient
            VoiceClient voiceClient = new VoiceClient(GetConfiguration());
            // 2. VerifyService Api parameters
            string phone_number = phoneNo;

            string message = "N'oubliez pas d'appeler votre mère pour son anniversaire demain.";
            string message_type = "ARN";
            Dictionary<string, string> voiceParams = new Dictionary<String, String>();
            voiceParams.Add("voice", "f-FR-fr");
            // 3. Make Voice call and get TeleSign Response
            TeleSignResponse response = voiceClient.Call(phone_number, message, message_type, voiceParams);
            // 4. Read TeleSign Response body.
            if ((response.StatusCode >= 200) && (response.StatusCode < 300))
            {
                Console.WriteLine("Call sent with message");
            }
        }        
    }
}
