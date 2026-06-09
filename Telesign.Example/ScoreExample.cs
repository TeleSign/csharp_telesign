using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Telesign.Example
{
    public class ScoreExample
    {
        private readonly string _ApiKey;
        private readonly string _CustomerId;
        private readonly string _PhoneNumber;
        private readonly string _EmailAddress;
        private readonly Dictionary<string, string> _OptionalParams;

        public ScoreExample(
            string apiKey,
            string customerId,
            string phoneNumber,
            string emailAddress,
            Dictionary<string, string>? optionalParams = null)
            {
                _ApiKey = apiKey;
                _CustomerId = customerId;
                _PhoneNumber = phoneNumber;
                _EmailAddress = emailAddress;
                _OptionalParams = optionalParams ?? new Dictionary<string, string>();
            }

        public void CheckPhoneNumberRiskLevel()
        {
            Console.WriteLine("*** Sending Score request to check phone number risk level ***");
            const string accountLifecycleEvent = "create";

            try
            {
                var scoreClient = new ScoreClient(_CustomerId, _ApiKey);
                var telesignResponse = scoreClient.Score(
                    _PhoneNumber,
                    accountLifecycleEvent,
                    _OptionalParams
                );

                if (telesignResponse != null && telesignResponse.OK && telesignResponse.Json != null)
                {
                    JObject json = telesignResponse.Json;
                    JObject? risk = (json["intelligence_details"]?["risk"] as JObject)
                        ?? (json["risk"] as JObject);

                    if (risk != null)
                    {
                        string level = risk["level"]?.ToString() ?? "unknown";
                        string recommendation = risk["recommendation"]?.ToString() ?? "none";

                        Console.WriteLine("Request successful.");
                        Console.WriteLine("Success from Score!!!");
                        Console.WriteLine("StatusCode: 200");
                        
                        Console.WriteLine($"Phone number: {_PhoneNumber}");
                        Console.WriteLine($"Risk level: {level}");
                        Console.WriteLine($"Recommendation: {recommendation}");
                        return;
                    }
                }

                Console.WriteLine("Risk information is not available or request failed.");
                PrintDetailedResponse(telesignResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }
        public void CheckEmailAddressRiskLevel()
        {
            Console.WriteLine("*** Sending Score request to check email address risk level ***");
            const string accountLifecycleEvent = "create";

            try
            {
                var scoreClient = new ScoreClient(_CustomerId, _ApiKey);
                var telesignResponse = scoreClient.EmailIntelligence(
                    _EmailAddress,
                    accountLifecycleEvent,
                    _OptionalParams
                );

                if (telesignResponse != null && telesignResponse.OK && telesignResponse.Json != null)
                {
                    JObject json = telesignResponse.Json;
                    JObject? risk = (json["intelligence_details"]?["risk"] as JObject)
                        ?? (json["risk"] as JObject);

                    if (risk != null)
                    {
                        string level = risk["level"]?.ToString() ?? "unknown";
                        string recommendation = risk["recommendation"]?.ToString() ?? "none";

                        Console.WriteLine("Request successful.");
                        Console.WriteLine("Success from Score!!!");
                        Console.WriteLine("StatusCode: 200");
                        
                        Console.WriteLine($"Email address: {_EmailAddress}");
                        Console.WriteLine($"Risk level: {level}");
                        Console.WriteLine($"Recommendation: {recommendation}");
                        return;
                    }
                }

                Console.WriteLine("Risk information is not available or request failed.");
                PrintDetailedResponse(telesignResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }

        private static void PrintDetailedResponse(RestClient.TelesignResponse? response)
        {
            if (response == null)
            {
                Console.WriteLine("Response is null.");
                return;
            }

            Console.WriteLine($"Status code: {response.StatusCode}");

            if (response.Json != null && response.Json.HasValues)
            {
                Console.WriteLine("JSON response:");
                Console.WriteLine(response.Json.ToString(Newtonsoft.Json.Formatting.Indented));
            }
            else if (!string.IsNullOrWhiteSpace(response.Body))
            {
                Console.WriteLine("Raw response body:");
                Console.WriteLine(response.Body);
            }
        }
    }
}
