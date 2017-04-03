//-----------------------------------------------------------------------
// <copyright file="Commands.cs" company="TeleSign Corporation">
//     Copyright (c) TeleSign Corporation 2012.
// </copyright>
// <license>MIT</license>
// <author>Zentaro Kavanagh</author>
//-----------------------------------------------------------------------

namespace TeleSign.TeleSignCmd
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using TeleSign.Services;
    using TeleSign.Services.PhoneId;
    using TeleSign.Services.Verify;
    using Services.Messaging;
    using Newtonsoft.Json.Linq;
    using Services.AutoVerify;
    using Services.Score;

    public class Commands
    {
        public static TeleSignServiceConfiguration GetConfiguration()
        {
            // By passing null (or not passing the parameter at all) to
            // the services config will be pulled from TeleSign.config.xml.
            // 
            // If you comment out return null, and uncomment the block 
            // below, then fill in your customer id and secret key
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

        [CliCommand(HelpString = "Help me")]
        public static void CheckPhoneTypeToBlockVoip(string[] args)
        {
            CheckArgument.ArrayLengthIs(args, 1, "args");         

            // 1. Initialize PhoneIdService
            PhoneIdService phoneId = new PhoneIdService(GetConfiguration());
            // 2. PhoneID Api parameters
            string phone_number = args[0];
            string phone_type_voip = "5";
            // 3. Fetch the response
            TSResponse response = phoneId.StandardLookup(phone_number);
            if (200 == response.StatusCode)
            {
                JObject jsonObject = response.JsonBody;
                JToken phone_type = jsonObject["phone_type"];
                if (phone_type["code"].ToString().Equals(phone_type_voip))
                    Console.WriteLine("Phone number {0} is a VOIP phone.", phone_number);
                else
                    Console.WriteLine("Phone number {0} is a NOT a VOIP phone.", phone_number);
            }
        }

        public static void Cleansing(string[] args)
        {
            CheckArgument.ArrayLengthIs(args, 1, "args");

            // 1. Initialize PhoneIdService
            PhoneIdService phoneId = new PhoneIdService(GetConfiguration());
            // 2. PhoneID Api parameters
            string phone_number = args[0];
            string extra_digit = "0";
            string incorrect_phone_number = phone_number + extra_digit;
            Dictionary<string, string> phoneIdParams = new Dictionary<string, string>();
            phoneIdParams.Add("account_lifecycle_event", "create");
            // 3. Fetch the response
            TSResponse response = phoneId.PhoneId(incorrect_phone_number, phoneIdParams);
            if (200 == response.StatusCode)
            {
                JObject jsonObject = response.JsonBody;

                JToken numbering = jsonObject["numbering"];
                JToken cleansing = numbering["cleansing"];
                JToken call = cleansing["call"];

                String country_code = call["country_code"].ToString();
                String cleansedNo = call["phone_number"].ToString();

                Console.WriteLine("Cleansed phone number has country code {0} and phone number is {1}.", country_code, cleansedNo);
                Console.WriteLine("Original phone number was {0}", numbering["original"]["complete_phone_number"].ToString());
            }
        }

        [CliCommand(HelpString = "Help me")]
        public static void CheckPhoneNumberDeactivated(string[] args)
        {
            CheckArgument.ArrayLengthIs(args, 1, "args");
            
            // 1. Initialize PhoneIdService
            PhoneIdService phoneId = new PhoneIdService(GetConfiguration());
            // 2. PhoneID Api parameters
            string phone_number = args[0];
            string ucid = "ATCK";
            // 3. Fetch the response
            TSResponse response = phoneId.NumberDeactivation(phone_number, ucid);
            if (200 == response.StatusCode)
            {
                JObject jsonObject = response.JsonBody;

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

        [CliCommand(HelpString = "Help me")]
        public static void CheckPhoneNumberRiskLevel(string[] args)
        {
            CheckArgument.ArrayLengthIs(args, 1, "args");

            // 1. Initialize PhoneIdService
            PhoneIdService phoneId = new PhoneIdService(GetConfiguration());
            // 2. PhoneID Api parameters
            string phone_number = args[0];
            string ucid = "BACF";
            // 3. Fetch the response
            TSResponse response = phoneId.ScoreLookup(phone_number, ucid);
            if (200 == response.StatusCode)
            {
                JObject jsonObject = response.JsonBody;

                JToken riskJsonObject = jsonObject["risk"];

                String level = riskJsonObject["level"].ToString();
                String recommendation = riskJsonObject["recommendation"].ToString();
                Console.WriteLine("Phone number {0} has a '{1}' risk level and the recommendation is to '{2}' the transaction.",
                        phone_number, level, recommendation);
            }
        }

        [CliCommand(HelpString = "Help me")]
        public static void CheckPhoneNoRiskLevel(string[] args)
        {
            CheckArgument.ArrayLengthIs(args, 1, "args");           

            // 1. Initialize PhoneIdService
            ScoreClient scoreClient = new ScoreClient(GetConfiguration());
            // 2. Score Api parameters
            string phone_number = args[0];
            string account_lifecycle_event = "create";

            // 3. Fetch the response
            TSResponse response = scoreClient.Score(phone_number, account_lifecycle_event);

            if (200 == response.StatusCode)
            {
                JObject jsonObject = response.JsonBody;
                String riskLevel = jsonObject["risk"]["level"].ToString();
                String recommendation = jsonObject["risk"]["recommendation"].ToString();
                Console.WriteLine("Phone number {0} has a '{1}' risk level and the recommendation is to '{2}' the transaction.", phone_number, riskLevel, recommendation);
            }
        }

        //[CliCommand(HelpString = "Help me")]
        //public static void RawPhoneIdContact(string[] args)
        //{
        //    CheckArgument.ArrayLengthIs(args, 1, "args");
        //    string phoneNumber = args[0];

        //    RawPhoneIdService service = new RawPhoneIdService(GetConfiguration());
        //    TSResponse jsonResponse = service.ContactLookupRaw(phoneNumber);

        //    Console.WriteLine(jsonResponse);
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void RawPhoneIdScore(string[] args)
        //{
        //    CheckArgument.ArrayLengthIs(args, 1, "args");
        //    string phoneNumber = args[0];

        //    RawPhoneIdService service = new RawPhoneIdService(GetConfiguration());
        //    TSResponse jsonResponse = service.ScoreLookupRaw(phoneNumber);

        //    Console.WriteLine(jsonResponse);
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void RawPhoneIdLive(string[] args)
        //{
        //    CheckArgument.ArrayLengthIs(args, 1, "args");
        //    string phoneNumber = args[0];

        //    RawPhoneIdService service = new RawPhoneIdService(GetConfiguration());
        //    TSResponse jsonResponse = service.LiveLookupRaw(phoneNumber);

        //    Console.WriteLine(jsonResponse);
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void PhoneIdStandard(string[] args)
        //{
        //    CheckArgument.ArrayLengthIs(args, 1, "args");
        //    string phoneNumber = args[0];

        //    PhoneIdService service = new PhoneIdService(GetConfiguration());
        //    TSResponse response = service.StandardLookup(phoneNumber);
        //}

        // Started from Here

        [CliCommand(HelpString = "Help me")]
        public static void GetStatusByExternalId(string[] args)
        {
            CheckArgument.ArrayLengthIs(args, 1, "args");
            // 1. Initialize AutoVerifyClient
            AutoVerifyClient autoVerifyClient = new AutoVerifyClient(GetConfiguration());
            // 2. AutoVerify Api parameters            
            string external_id = "external_id";
            // 3. Make Messaging API call get TeleSign Response
            TSResponse response = autoVerifyClient.Status(external_id);
            // 4. Read TeleSign Response body.
            if (200 == response.StatusCode)
            {
                JObject teleSignJsonObject = response.JsonBody;
                JToken statusObject = teleSignJsonObject["status"];
                Console.WriteLine("AutoVerify transaction with external_id as {0} has status code  as {1} and status description as {2}.", external_id, statusObject["code"].ToString(), statusObject["description"].ToString());
            }
        }

        [CliCommand(HelpString = "Help me")]
        public static void MessagingExample(string[] args)
        {
            CheckArgument.ArrayLengthIs(args, 1, "args");            
            // 1. Initialize MessagingClient
            MessagingClient messagingClient = new MessagingClient(GetConfiguration());
            // 2. Messaging Api parameters
            string phone_number = args[0];
            string message = "You're scheduled for a dentist appointment at 2:30PM.";
            string messageType = "ARN";
            // 3. Make Messaging API call get TeleSign Response
            TSResponse response = messagingClient.MessageRaw(phone_number, message, messageType);
            // 4. Read TeleSign Response body.
            if (200 == response.StatusCode)
            {
                JObject teleSignJsonObject = response.JsonBody;
                string reference_id = teleSignJsonObject["reference_id"].ToString();
                // 1.1 Get the reference ID/
                Console.WriteLine(reference_id);
            }
        }

        [CliCommand(HelpString = "Help me")]
        public static void MessagingExampleWithCode(string[] args)
        {
            CheckArgument.ArrayLengthIs(args, 1, "args");
            // 1. Initialize MessagingClient
            MessagingClient messagingClient = new MessagingClient(GetConfiguration());
            // 2. Messaging Api parameters
            string phone_number = args[0];
            string message = "Your code is ";
            string messageType = "OTP";
            // 2.1 Generating 5 digit random no to send
            Random r = new Random();
            string verifyCode = r.Next(10000, 99999).ToString();
            // 3. Make Messaging API call get TeleSign Response
            TSResponse response = messagingClient.MessageRaw(phone_number, message + verifyCode, messageType);
            // 4. Read TeleSign Response body.
            if (200 == response.StatusCode)
            {   
                Console.WriteLine("Please enter the verification code you were sent: ");
                string user_entered_verify_code = Console.ReadLine();
                if (user_entered_verify_code.Equals(verifyCode))
                    Console.WriteLine("Your code is correct.");
                else
                    Console.WriteLine("Your code is not correct.");
            }
        }

        //[CliCommand(HelpString = "Help me")]
        //public static void PhoneIdScore(string[] args)
        //{
        //    CheckArgument.ArrayLengthIs(args, 1, "args");
        //    string phoneNumber = args[0];

        //    PhoneIdService service = new PhoneIdService(GetConfiguration());
        //    TSResponse response = service.ScoreLookup(phoneNumber);

        //    Console.WriteLine("Phone Number: {0}", phoneNumber);
        //    Console.WriteLine("Risk        : {0} [{1}] - Recommend {2}", response.Risk.Level, response.Risk.Score, response.Risk.Recommendation);
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void PhoneIdLive(string[] args)
        //{
        //    CheckArgument.ArrayLengthIs(args, 1, "args");
        //    string phoneNumber = args[0];

        //    PhoneIdService service = new PhoneIdService(GetConfiguration());
        //    TSResponse response = service.LiveLookup(phoneNumber);

        //    Console.WriteLine("Phone Number      : {0}", phoneNumber);
        //    Console.WriteLine("Subscriber Status : {0}", response.Live.SubscriberStatus);
        //    Console.WriteLine("Device            : {0}", response.Live.DeviceStatus);
        //    Console.Write("Roaming           : {0}", response.Live.Roaming);

        //    if (string.IsNullOrEmpty(response.Live.RoamingCountry))
        //    {
        //        Console.WriteLine();
        //    }
        //    else
        //    {
        //        Console.WriteLine(" in {0}", response.Live.RoamingCountry);
        //    }
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void PhoneIdContact(string[] args)
        //{
        //    CheckArgument.ArrayLengthIs(args, 1, "args");
        //    string phoneNumber = args[0];

        //    PhoneIdService service = new PhoneIdService(GetConfiguration());
        //    PhoneIdContactResponse response = service.ContactLookup(phoneNumber);

        //    Console.WriteLine("Phone Number: {0}", phoneNumber);
        //    Console.WriteLine("Name        : {0}", response.Contact.FullName);
        //    Console.WriteLine("Address     :\r\n{0}", response.Contact.GetFullAddress());
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void MapRegistrationLocation(string[] args)
        //{
        //    CheckArgument.ArrayLengthIs(args, 1, "args");
        //    string phoneNumber = args[0];

        //    PhoneIdService service = new PhoneIdService(GetConfiguration());
        //    PhoneIdStandardResponse response = service.StandardLookup(phoneNumber);

        //    string url = string.Format(
        //                "http://maps.google.com/maps?q={0},{1}", 
        //                response.Location.Coordinates.Latitude, 
        //                response.Location.Coordinates.Longitude);

        //    Process.Start(url);
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void MapContactLocation(string[] args)
        //{
        //    CheckArgument.ArrayLengthIs(args, 1, "args");
        //    string phoneNumber = args[0];

        //    PhoneIdService service = new PhoneIdService(GetConfiguration());
        //    PhoneIdContactResponse response = service.ContactLookup(phoneNumber);

        //    string address = response.Contact.GetFullAddressOnSingleLine();
        //    Console.WriteLine("Google Mapping: {0}", address);

        //    string url = string.Format(
        //                "http://maps.google.com/maps?q={0}",
        //                address);

        //    Process.Start(url);
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void VerifySms(string[] args)
        //{
        //    PerformVerify(args, VerificationMethod.Sms);
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void VerifyTwoWaySms(string[] args)
        //{
        //    PerformVerify(args, VerificationMethod.TwoWaySms);
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void VerifyCall(string[] args)
        //{
        //    PerformVerify(args, VerificationMethod.Call);
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void VerifyPush(string[] args)
        //{
        //    PerformVerify(args, VerificationMethod.Push);
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void SendSms(string[] args)
        //{
        //    CheckArgument.ArrayLengthAtLeast(args, 1, "args");

        //    string phoneNumber = args[0];

        //    string code = null;
        //    if (args.Length >= 2)
        //    {
        //        code = args[1];
        //    }

        //    string language = "en";
        //    if (args.Length >= 3)
        //    {
        //        language = args[2];
        //    }

        //    try
        //    {
        //        VerifyService verify = new VerifyService(GetConfiguration());
        //        VerifyResponse verifyResponse = null;
        //        verifyResponse = verify.SendSms(phoneNumber, code, string.Empty, language);
        //        Console.WriteLine("Sent sms");
        //    }
        //    catch (Exception x)
        //    {
        //        Console.WriteLine("Error: " + x.ToString());
        //    }
        //}

        //[CliCommand(HelpString = "Help me")]
        //public static void SendTwoWaySms(string[] args)
        //{
        //    CheckArgument.ArrayLengthAtLeast(args, 1, "args");

        //    string phoneNumber = args[0];
        //    string message = string.Empty;

        //    if (args.Length >= 2)
        //    {
        //        message = args[1];
        //    }

        //    try
        //    {
        //        VerifyService verify = new VerifyService(GetConfiguration());
        //        VerifyResponse verifyResponse = null;
        //        verifyResponse = verify.SendTwoWaySms(phoneNumber, message);
        //        Console.WriteLine("Sent two way sms");
        //    }
        //    catch (Exception x)
        //    {
        //        Console.WriteLine("Error: " + x.ToString());
        //    }
        //}

        //private static void PerformVerify(string[] args, VerificationMethod method)
        //{
        //    CheckArgument.ArrayLengthAtLeast(args, 1, "args");

        //    string phoneNumber = args[0];

        //    string code = null;
        //    if (args.Length >= 2)
        //    {
        //        code = args[1];
        //    }

        //    string language = "en";
        //    if (args.Length >= 3)
        //    {
        //        language = args[2];
        //    }

        //    VerifyService verify = new VerifyService(GetConfiguration());
        //    VerifyResponse verifyResponse = null;

        //    if (method == VerificationMethod.Sms)
        //    {
        //        verifyResponse = verify.SendSms(phoneNumber, code, string.Empty, language);
        //    }
        //    else if (method == VerificationMethod.Call)
        //    {
        //        verifyResponse = verify.InitiateCall(phoneNumber, code, language);
        //    }
        //    else if (method == VerificationMethod.Push)
        //    {
        //        verifyResponse = verify.InitiatePush(phoneNumber, code);
        //    }
        //    else if (method == VerificationMethod.TwoWaySms)
        //    {
        //        verifyResponse = verify.SendTwoWaySms(phoneNumber);
        //    }
        //    else
        //    {
        //        throw new NotImplementedException("Invalid verification method");
        //    }

        //    foreach (TeleSignApiError e in verifyResponse.Errors)
        //    {
        //        Console.WriteLine(
        //                    "ERROR: [{0}] - {1}", 
        //                    e.Code, 
        //                    e.Description);
        //    }

        //    while (true)
        //    {
        //        Console.Write("Enter the code sent to phone [Just <enter> checks status. 'quit' to exit]: ");
        //        string enteredCode = Console.ReadLine();

        //        if (enteredCode.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            break;
        //        }

        //        Console.WriteLine(string.IsNullOrEmpty(enteredCode) ? "Checking status..." : "Validating code...");

        //        VerifyResponse statusResponse = verify.ValidateCode(
        //                    verifyResponse.ReferenceId,
        //                    enteredCode);

        //        Console.WriteLine(
        //                    "Transaction Status: {0} -- {1}\r\nCode State: {2}",
        //                    statusResponse.Status.Code,
        //                    statusResponse.Status.Description,
        //                    (statusResponse.VerifyInfo != null) 
        //                                ? statusResponse.VerifyInfo.CodeState.ToString()
        //                                : "Not Sent");

        //        if ((statusResponse.VerifyInfo != null) && (statusResponse.VerifyInfo.CodeState == CodeState.Valid))
        //        {
        //            Console.WriteLine("Code was valid. Exiting.");
        //            break;
        //        }
        //    }
        //}
    }
}
