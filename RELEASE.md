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