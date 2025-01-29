using System;

namespace Telesign.Example;

public class VoiceExample(string apiKey, string customerId, string phoneNumber)
{
    private readonly string _ApiKey = apiKey;
    private readonly string _CustomerId = customerId;
    private readonly string _PhoneNumber = phoneNumber;

    public void SendVoiceCall()
    {
        Console.WriteLine("***Send voice call***");
        string message = "You're scheduled for a dentist appointment at 2:30PM.";
        string messageType = "ARN";

        try
        {
            VoiceClient voiceClient = new(_CustomerId, _ApiKey);
            RestClient.TelesignResponse telesignResponse = voiceClient.Call(_PhoneNumber, message, messageType);
            Console.WriteLine($"Http Status code: {telesignResponse.StatusCode}");
            Console.WriteLine($"Response body: {Environment.NewLine + telesignResponse.Body}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void SendVoiceCallFrench()
    {
        Console.WriteLine("***Send voice call french language***");
        string message = "N'oubliez pas d'appeler votre mère pour son anniversaire demain.";
        string messageType = "ARN";

        Dictionary<string, string> parameters = new()
        {
            {"voice", "f-FR-fr" }
        };

        try
        {
            VoiceClient voiceClient = new(_CustomerId, _ApiKey);
            RestClient.TelesignResponse telesignResponse = voiceClient.Call(_PhoneNumber, message, messageType, parameters);
            Console.WriteLine($"Http Status code: {telesignResponse.StatusCode}");
            Console.WriteLine($"Response body: {Environment.NewLine + telesignResponse.Body}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void SendVoiceCallWithVerificationCode()
    {
        Console.WriteLine("***Send voice call with verification code***");
        string verifyCode = "12345";
        string message = string.Format("Hello, your code is {0}. Once again, your code is {1}. Goodbye.", verifyCode, verifyCode);
        string messageType = "OTP";

        try
        {
            VoiceClient voiceClient = new(_CustomerId, _ApiKey);
            RestClient.TelesignResponse telesignResponse = voiceClient.Call(_PhoneNumber, message, messageType);
            Console.WriteLine($"Http Status code: {telesignResponse.StatusCode}");
            Console.WriteLine($"Response body: {Environment.NewLine + telesignResponse.Body}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
