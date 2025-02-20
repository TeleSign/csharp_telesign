using System;

namespace Telesign.Example;

public class ScoreExample(string apiKey, string customerId, string phoneNumber)
{
    private readonly string _ApiKey = apiKey;
    private readonly string _CustomerId = customerId;
    private readonly string _PhoneNumber = phoneNumber;

    public void CheckPhoneNumberRiskLevel()
    {
        Console.WriteLine("***Send score request by checking phone number risk level***");
        string accountLifecycleEvent = "create";

        try
        {
            ScoreClient scoreClient = new(_CustomerId, _ApiKey);
            RestClient.TelesignResponse telesignResponse = scoreClient.Score(_PhoneNumber, accountLifecycleEvent);

            if (telesignResponse.OK)
                Console.WriteLine(string.Format("Phone number {0} has a '{1}' risk level and the recommendation is to '{2}' the transaction.",
                    _PhoneNumber,
                    telesignResponse.Json["risk"]["level"],
                    telesignResponse.Json["risk"]["recommendation"]));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
