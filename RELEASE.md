3.0.0
- Removed all functionality and methods for App Verify SDK (Android) from the C# SS SDK

2.6.0
- Added PATCH method to RestClient.cs file
- Updated version in the Telesign.csproj

2.5.0
- Added tracking to requests
- Updated instructions in README file to install this SDK by default using the latest version.

2.4.0
- Added supported versions of .NET Core 6, 7, 8, and 9
- Updated syntax code to support .NET framework and .Net core

2.3.0
- Moved support for application/json content-type from PhoneIdClient class to RestClient class
- Added two testing post method for application json test.

2.2.1
- Added support for application/json content-type

2.2.0
- AutoVerify rebranded to AppVerify, please update your integration.

2.1.0
- Major refactor and simplification into generic REST client.
- updated and improved README
- added appveyor CI, codecov coverage and additional unit tests
- API parameters are now passed as a dictionary to endpoint handlers.
- UserAgent is now set to track client usage and help debug issues.
- GenerateTelesignHeaders is now static and easily extracted from the SDK if
  custom behavior/implementation is required.

1.0.0
- Initial release