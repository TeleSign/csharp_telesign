========
TeleSign
========

TeleSign provides the world’s most comprehensive approach to account security for Web and mobile applications.
For more information about TeleSign, visit the `TeleSign website <http://www.TeleSign.com>`_.

**Author**: Telesign Corp.

TeleSign Web Services: .NET SDK
---------------------------------

**TeleSign web services** conform to the `REST Web Service Design Model <http://en.wikipedia.org/wiki/Representational_state_transfer>`_. Services are exposed as URI-addressable resources through the set of *RESTful* procedures in our **TeleSign REST API**.

The **TeleSign .NET SDK** is a Microsoft .NET component that provides an interface to `TeleSign web services <https://developer.telesign.com/docs/getting-started-with-the-rest-api>`_. 

It contains a .NET Framework class library that presents our web services in an intuitive, hierarchical object model, so you can create and manipulate them in the way you're accustomed to. You can use this SDK to build TeleSign‑based .NET applications.

Documentation
-------------

Detailed documentation for TeleSign REST APIs is available in the `Developer Portal <https://developer.telesign.com/>`_.

Authentication
--------------

**You will need a Customer ID and API Key in order to use TeleSign’s REST API**.  If you are already a customer and need an API Key, you can generate one in `TelePortal <https://teleportal.telesign.com>`_.  If you are not a customer and would like to get an API Key, please contact `support@telesign.com <mailto:support@telesign.com>`_.

You supply your credentials to the API either by editing the TeleSign.config.xml file and filling in the CustomerId and
SecretKey values or you can create the credentials in code. Passing null to the service constructors uses the file.

**Option 1. Supply TeleSign.config.xml**

>>>
  <?xml version="1.0" encoding="utf-8" ?>
  <TeleSignConfig>
  <ServiceUri>https://rest.telesign.com</ServiceUri>
  <Accounts>
    <Account name="default">
      <!-- Enter your customer id and secret key here. -->
      <CustomerId>99999999-9999-9999-9999-000000000000</CustomerId>
      <SecretKey>xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx==</SecretKey>
    </Account>
  </Accounts>
 </TeleSignConfig>


**Option 2. Supply the credential with code**

>>>
  Guid customerId = Guid.Parse("*** Customer ID goes here ***");
  string secretKey = "*** Secret Key goes here ***";
  TeleSignCredential credential = new TeleSignCredential(
              customerId,
              secretKey);
  TeleSignServiceConfiguration config = new TeleSignServiceConfiguration(credential);
  // For Verify Sms or Call Service
  VerifyService verify = new VerifyService(config);
  // For PhoneId products
  PhoneIdServer phoneId = PhoneIdService(config);


Code Example: Messaging Example
------------------------------------
These examples assume you are using the file for authentication/configuration described above.

>>>
string phoneNumber = "+1 555-555-5555";
string message = "You're scheduled for a dentist appointment at 2:30PM.";
string messageType = "ARN";
MessagingClient messagingClient = new MessagingClient();
TeleSignResponse response = messagingClient.Message(phone_number, message, messageType);
if (((int)response.StatusCode) >= 200 && ((int)response.StatusCode) < 300)
            {
                JObject teleSignJsonObject = response.Json;
                string reference_id = teleSignJsonObject["reference_id"].ToString();                
                Console.WriteLine(reference_id);
            }

For more examples, see the documentation or browse the examples source code in 
**Example.cs** in the **TeleSign.Example.ConsoleApplication** project.


Support and Feedback
--------------------

For more information, please contact your TeleSign representative:

Email: `support@telesign.com <mailto:support@telesign.com>`_

