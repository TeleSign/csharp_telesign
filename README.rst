﻿.. image:: https://raw.github.com/TeleSign/csharp_telesign/master/csharp_banner.jpg
    :target: https://developer.telesign.com

.. image:: https://img.shields.io/appveyor/ci/telesign/csharp-telesign.svg
    :target: https://ci.appveyor.com/project/TeleSign/csharp-telesign

.. image:: https://img.shields.io/codecov/c/github/TeleSign/csharp_telesign.svg
    :target: https://codecov.io/gh/TeleSign/csharp_telesign

.. image:: https://img.shields.io/nuget/v/Telesign.svg
    :target: https://www.nuget.org/packages/Telesign

.. image:: https://img.shields.io/github/license/TeleSign/csharp_telesign.svg
    :target: https://github.com/TeleSign/csharp_telesign/blob/master/LICENSE.txt

===============
TeleSign C# SDK
===============

TeleSign is a communications platform as a service (CPaaS) company, founded on security. Since 2005, TeleSign has
been a trusted partner to the world’s leading websites and mobile applications, helping secure billions of end-user
accounts. Today, TeleSign’s data-driven, cloud communications platform is changing the way businesses engage with
customers and prevent fraud.

For more information about TeleSign, visit our `website <http://www.TeleSign.com>`_.

Documentation
-------------

Code documentation is included in the SDK. Complete documentation, quick start guides and reference material
for the TeleSign API is available within the `TeleSign Standard Documentation <https://standard.telesign.com/>`_.

Installation
------------

To add the TeleSign C# SDK to your project, add Telesign to your packages.config:

.. code-block:: xml

    <package id="Telesign" version="(insert latest version)" targetFramework="(insert framework)" />

.NET framework **v4.5.2+** is required for the TeleSign C# SDK.

Authentication
--------------

You will need a Customer ID and API Key in order to use TeleSign’s API. If you already have an account you can retrieve
them from your account dashboard within the `Portal <https://portal.telesign.com/login>`_. If you have not signed up
yet, sign up `here <https://portal.telesign.com/signup>`_.

Dependencies
------------

We make use of popular, feature-rich and well-tested open-source libraries to perform the underlying functionality of
the SDK. These dependencies are managed by the community accepted package manager. If you are unable to add these
additional third party dependencies to your project we have ensured that the SDK code is easy to read and can serve as
sample code. We have also made sure that more complicated functions such as GenerateTelesignHeaders can be easily
extracted from the SDK and used 'as is' in your project.

C# Code Example: Messaging
--------------------------

Here is a basic code example with JSON response.

.. code-block:: java

    string customerId = "FFFFFFFF-EEEE-DDDD-1234-AB1234567890";
    string apiKey = "EXAMPLETE8sTgg45yusumoN6BYsBVkh+yRJ5czgsnCehZaOYldPJdmFh6NeX8kunZ2zU1YWaUw/0wV6xfw==";

    string phoneNumber = "phone_number";
    string message = "You're scheduled for a dentist appointment at 2:30PM.";
    string messageType = "ARN";

    MessagingClient messagingClient = new MessagingClient(customerId, apiKey);
    RestClient.TelesignResponse telesignResponse = messagingClient.message(phoneNumber, message, messageType);

.. code-block:: javascript
    
    {'reference_id': 'DGFDF6E11AB86303ASDFD425BE00000657',
     'status': {'code': 103,
        'description': 'Call in progress',
        'updated_on': '2016-12-12T00:39:58.325559Z'}}

For more examples, see the
`examples <https://github.com/TeleSign/csharp_telesign/tree/master/Telesign.Example>`_ project or visit
the `TeleSign Standard Documentation <https://standard.telesign.com/>`_.
