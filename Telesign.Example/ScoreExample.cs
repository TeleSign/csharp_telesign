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
        private readonly Dictionary<string, string> _OptionalParams;

        public ScoreExample(
            string apiKey,
            string customerId,
            string phoneNumber,
            Dictionary<string, string>? optionalParams = null)
            {
                _ApiKey = apiKey;
                _CustomerId = customerId;
                _PhoneNumber = phoneNumber;
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
                    JObject? risk = json["risk"] as JObject;

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
                if (telesignResponse != null)
                    Console.WriteLine($"Status code: {telesignResponse.StatusCode}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }
    }
}
