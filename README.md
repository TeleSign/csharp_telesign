[![NuGet](https://img.shields.io/nuget/v/Telesign.svg)](https://www.nuget.org/packages/Telesign) [![license](https://img.shields.io/pypi/l/telesign.svg)](https://github.com/TeleSign/python_telesign/blob/master/LICENSE.txt)

# Telesign Self-service C# SDK

[Telesign](https://telesign.com) connects, protects, and defends the customer experience with intelligence from billions of digital interactions and mobile signals. Through developer-friendly APIs that deliver user verification, digital identity, and omnichannel communications, we help the world's largest brands secure onboarding, maintain account integrity, prevent fraud, and streamline omnichannel engagement.

## Supported .NET versions
* .NET Framework 4.8+ 
* .NET Core 6, 7, 8, and 9

## Requirements

* **dotnet CLI** *(Optional)* - This tool isn't required to use this SDK, but it is required to use the installation instructions below.  

> **NOTE:**
>
> These instructions are for MacOS. They will need to be adapted if you are installing on Windows.

## Installation

Follow these steps to add this SDK as a dependency to your project.

1. *(Optional)* Create a new directory for your C# project. Skip this step if you already have created a project. If you plan to create multiple C# projects that use Telesign, we recommend that you group them within a `telesign_integrations` directory.

```
    cd ~/code/local
    mkdir telesign_integrations
    cd telesign_integrations
    mkdir {your project name}
    cd {your project name}
```

2. Create a new C# solution using the dotnet CLI in the top-level directory of your project.

   `dotnet new console -o {name of the solution}`

   For example:

    `dotnet new console -o SendSMS`

3. Install the Telesign SDK using the dotnet CLI. 

   `dotnet add package Telesign`

   Once the SDK is installed, you should see a message in the terminal notifying you that you have successfully installed the SDK.

> **NOTE:**
> 
> There are alternate ways of installing this SDK. For examples, see this [SDK's page](https://www.nuget.org/packages/Telesign) in the NuGet gallery.

4. Install the [Newtonsoft.json](https://www.nuget.org/packages/Newtonsoft.Json/) dependency using the dotnet CLI.

   `dotnet add package Newtonsoft.Json --version 13.*`


## Authentication

If you use a Telesign SDK to make your request, authentication is handled behind-the-scenes for you. All you need to provide is your Customer ID and API Key. The SDKs apply Digest authentication whenever they make a request to a Telesign service where it is supported. When Digest authentication is not supported, the SDKs apply Basic authentication.

## What's next

* Learn to send a request to Telesign with code with one of our [tutorials](https://developer.telesign.com/enterprise/docs/tutorials).  
* Browse our [Developer Portal](https://developer.telesign.com) for tutorials, how-to guides, reference content, and more.
* Check out our [sample code](https://github.com/TeleSign/sample_code) on GitHub.
