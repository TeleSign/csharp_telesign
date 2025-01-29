using System;

namespace Telesign.Example;

public class MessagingExample(string apiKey, string customerId, string phoneNumber)
{
    private readonly string _ApiKey = apiKey;
    private readonly string _CustomerId = customerId;
    private readonly string _PhoneNumber = phoneNumber;

    public void SendMessage()
    {
        Console.WriteLine("***Send message***");
        string message = "You're scheduled for a dentist appointment at 2:30PM.";
        string messageType = "ARN";
        try
        {
            MessagingClient messagingClient = new(_CustomerId, _ApiKey);
            RestClient.TelesignResponse telesignResponse = messagingClient.Message(_PhoneNumber, message, messageType);
            Console.WriteLine($"Http Status code: {telesignResponse.StatusCode}");
            Console.WriteLine($"Response body: {Environment.NewLine + telesignResponse.Body}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void SendMessageWithVerificationCode()
    {
        Console.WriteLine("***Send message with verification code***");
        string verifyCode = "12345";
        string message = string.Format("Your code is {0}", verifyCode);
        string messageType = "OTP";

        try
        {
            MessagingClient messagingClient = new (_CustomerId, _ApiKey);
            RestClient.TelesignResponse telesignResponse = messagingClient.Message(_PhoneNumber, message, messageType);
            Console.WriteLine($"Http Status code: {telesignResponse.StatusCode}");
            Console.WriteLine($"Response body: {Environment.NewLine + telesignResponse.Body}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
