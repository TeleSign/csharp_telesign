using System;

namespace Telesign.Example;

public class PhoneIdExample(string apiKey, string customerId, string phoneNumber)
{
    private readonly string _ApiKey = apiKey;
    private readonly string _CustomerId = customerId;
    private readonly string _PhoneNumber = phoneNumber;

    public void SendRequestWithAddons()
    {
        Console.WriteLine("***Send request Phone Id with 'addons' parameter***");
        Dictionary<string, object> contact = [];
        contact.Add("contact", new Dictionary<string, object>());
        Dictionary<string, object> parameters = [];
        parameters.Add("addons", contact);
        try
        {
            PhoneIdClient phoneIdClient = new(_CustomerId, _ApiKey);
            RestClient.TelesignResponse telesignResponse = phoneIdClient.PhoneId(_PhoneNumber, parameters);

            if (telesignResponse.OK)
                Console.WriteLine(telesignResponse.Json);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void CheckPhoneTypeToBlockVoip()
    {
        Console.WriteLine("***Send request phone id by checking phone type to block voip***");
        string phoneTypeVoip = "5";

        try
        {
            PhoneIdClient phoneIdClient = new(_CustomerId, _ApiKey);
            RestClient.TelesignResponse telesignResponse = phoneIdClient.PhoneId(_PhoneNumber);

            if (telesignResponse.OK)
                if (telesignResponse.Json["phone_type"]["code"].ToString() == phoneTypeVoip)
                    Console.WriteLine(string.Format("Phone number {0} is a VOIP phone.", _PhoneNumber));
                else
                    Console.WriteLine(string.Format("Phone number {0} is not a VOIP phone.", _PhoneNumber));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void CleanPhoneNumber()
    {
        Console.WriteLine("***Send request Phone Id by cleasing phone***");
        string extraDigit = "0";
        string incorrectPhoneNumber = string.Format("{0}{1}", _PhoneNumber, extraDigit);

        try
        {
            PhoneIdClient phoneIdClient = new(_CustomerId, _ApiKey);
            RestClient.TelesignResponse telesignResponse = phoneIdClient.PhoneId(incorrectPhoneNumber);

            if (telesignResponse.OK)
            {
                Console.WriteLine(string.Format("Cleansed phone number has country code {0} and phone number is {1}.",
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
    }
}
