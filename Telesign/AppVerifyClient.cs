using System.Net;

namespace Telesign
{
    /// <summary>
    ///  AppVerify is a secure, lightweight SDK that integrates a frictionless user verification process into existing native mobile applications.
    /// </summary>
    public class AppVerifyClient : RestClient
    {
        public AppVerifyClient(string customerId,
                                string apiKey)
            : base(customerId,
                   apiKey)
        { }

        public AppVerifyClient(string customerId,
                                string apiKey,
                                string restEndPoint,
                                string source,
                                string sdkVersionOrigin,
                                string sdkVersionDependency)
            : base(customerId,
                   apiKey,
                   restEndPoint,
                   source: source,
                   sdkVersionOrigin: sdkVersionOrigin,
                   sdkVersionDependency: sdkVersionDependency)
        { }

        public AppVerifyClient(string customerId,
                                string apiKey,
                                string restEndPoint,
                                int timeout,
                                WebProxy proxy,
                                string proxyUsername,
                                string proxyPassword,
                                string source,
                                string sdkVersionOrigin,
                                string sdkVersionDependency)
            : base(customerId,
                   apiKey,
                   restEndPoint,
                   timeout,
                   proxy,
                   proxyUsername,
                   proxyPassword,
                   source,
                   sdkVersionOrigin,
                   sdkVersionDependency)
        {
        }
    }
}
