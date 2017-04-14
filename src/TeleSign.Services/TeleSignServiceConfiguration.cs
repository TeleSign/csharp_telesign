//-----------------------------------------------------------------------
// <copyright file="TeleSignServiceConfiguration.cs" company="TeleSign Corporation">
//     Copyright (c) TeleSign Corporation 2012.
// </copyright>
// <license>MIT</license>
// <author>Zentaro Kavanagh</author>
//-----------------------------------------------------------------------

namespace TeleSign.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// The information required to connect to the TeleSign REST API. This
    /// includes a credential containing a customer id and secret key and
    /// the Uri to connect to the service. In general there is no
    /// need to supply or modify the URL as the default value will
    /// point to the TeleSign service.
    /// </summary>
    public class TeleSignServiceConfiguration
    {
        /// <summary>
        /// The address to connect to.
        /// </summary>
        public const string DefaultServiceAddress = "https://rest.telesign.com/";
        public const string DefaultServiceMobileAddress = "https://rest-mobile.telesign.com/";
        /// <summary>
        /// TeleSign Proxy Parameters
        /// </summary>
        public Dictionary<String, String> TeleSignServiceParams { get; set; }

        /// <summary>
        /// Initializes a new instance of the TeleSignServiceConfiguration class
        /// with a supplied credential and service address URI. Most of the time
        /// you should use the other constructor which doesn't take the URI
        /// parameter.
        /// </summary>
        /// <param name="credential">
        /// The TeleSign credentials supplied to you for authenticating to
        /// Telesign services.
        /// </param>
        /// <param name="serviceAddress">
        /// The base URI of the TeleSign service. Most of the time you should not
        /// need to provide this.
        /// </param>
        public TeleSignServiceConfiguration(
                    TeleSignCredential credential,
                    Uri serviceAddress,
            		Uri serviceMobileAddress)
        {
            this.Credential = credential;
            this.ServiceAddress = serviceAddress;
            this.ServiceMobileAddress = serviceMobileAddress;            
        }
        /// <summary>
        /// Initializes a new instance of the TeleSignServiceConfiguration class
        /// with a supplied credential and service address URI. Most of the time
        /// you should use the other constructor which doesn't take the URI
        /// parameter.
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="TeleSignServiceParams"></param>
        public TeleSignServiceConfiguration(
                    TeleSignCredential credential,
                    Dictionary<string, string> TeleSignServiceParams)
        {
            this.Credential = credential;            
            this.TeleSignServiceParams = TeleSignServiceParams;
        }

        /// <summary>
        /// Initializes a new instance of the TeleSignServiceConfiguration class
        /// with a supplied credential. The default URI contained in the constant
        /// TeleSignServiceConfiguration.DefaultServiceAddress will be used.
        /// </summary>
        /// <param name="credential">
        /// The TeleSign credentials supplied to you for authenticating to
        /// Telesign services.
        /// </param>
        public TeleSignServiceConfiguration(TeleSignCredential credential)
            : this(
                    credential,
                    new Uri(TeleSignServiceConfiguration.DefaultServiceAddress),
            		new Uri(TeleSignServiceConfiguration.DefaultServiceMobileAddress))
        {
        }

        /// <summary>
        /// Gets or sets the TeleSign credentials used in this configuration.
        /// </summary>
        /// <value>The TeleSign credentials used in this configuration.</value>
        public TeleSignCredential Credential { get; set; }

        /// <summary>
        /// Gets or sets the base URI of the telesign REST services.
        /// </summary>
        /// <value>The base URI of the telesign REST services.</value>
        public Uri ServiceAddress { get; set; }
		public Uri ServiceMobileAddress { get; set; }

        /// <summary>
        /// This reads a configuration file called TeleSign.config.xml from
        /// the same directory as this dll.
        /// </summary>
        /// <returns>An instantiated TeleSignServiceConfiguration object containing configuration.</returns>
        public static TeleSignServiceConfiguration ReadConfigurationFile(string accountName = "default")
        {
            string configFilePath = TeleSignServiceConfiguration.GetDefaultConfigurationFilePath();

            return TeleSignServiceConfiguration.ReadConfigurationFile(configFilePath, accountName);
        }

        public static List<string> GetAccountNames()
        {
            string configFilePath = TeleSignServiceConfiguration.GetDefaultConfigurationFilePath();

            return TeleSignServiceConfiguration.GetAccountNames(configFilePath);
        }

        public static List<string> GetAccountNames(string configFilePath)
        {
            XDocument doc = XDocument.Load(configFilePath);

            XElement root = doc.Element("TeleSignConfig");
            string serviceUri = (string)root.Element("ServiceUri");

            List<string> names = new List<string>();

            foreach (XElement account in root.Element("Accounts").Elements("Account"))
            {
                names.Add((string)account.Attribute("name"));
            }

            return names;
        }

        /// <summary>
        /// This reads a configuration file from a specific file location.
        /// </summary>
        /// <param name="configFilePath">The path to the configuration file.</param>
        /// <returns>An instantiated TeleSignServiceConfiguration object containing configuration.</returns>
        public static TeleSignServiceConfiguration ReadConfigurationFile(string configFilePath, string accountName = "default")
        {
            Dictionary<string, string> teleSignConfig = new Dictionary<string, string>();
            XDocument doc = XDocument.Load(configFilePath);

            XElement root = doc.Element("TeleSignConfig");
            string serviceUri = (string)root.Element("ServiceUri");
            string serviceMobileUri = (string)root.Element("ServiceMobileUri");

            // Reading Proxy Settings
            XElement proxy = root.Element("Proxy");
            // if proxy enabled
            if ((Boolean)proxy.Attribute("enabled")) {
                //Dictionary<String, String> teleSignParams = new Dictionary<string, string>();
                teleSignConfig.Add("HttpProxyIPAddress", (string)proxy.Element("HttpProxyIPAddress"));
                teleSignConfig.Add("HttpProxyPort", (string)proxy.Element("HttpProxyPort"));                
                // and if proxy authentication enabled
                if ((Boolean)proxy.Element("HttpProxyAuthentication").Attribute("enabled")) {
                    XElement httpProxyAuthentication = proxy.Element("HttpProxyAuthentication");
                    teleSignConfig.Add("HttpProxyUsername", (string)httpProxyAuthentication.Element("HttpProxyUsername"));
                    teleSignConfig.Add("HttpProxyPassword", (string)httpProxyAuthentication.Element("HttpProxyPassword"));                    
                    //Console.WriteLine("Proxy url {0}:{1}/{2}:{3}", httpProxyIPAddress, httpProxyPort, httpProxyUsername, HttpProxyPassword);
                }
            }

            foreach (XElement account in root.Element("Accounts").Elements("Account"))
            {
                if (((string)account.Attribute("name")).ToLowerInvariant() == accountName.ToLowerInvariant())
                {
                    string overrideServiceUri = (string)account.Element("ServiceUri");
                    string overrideServiceMobileUri = (string)account.Element("ServiceMobileUri");

                    if (overrideServiceUri != null)
                    {
                        serviceUri = overrideServiceUri;                        
                    }
                    
                    if (overrideServiceMobileUri != null)
                    {
                        serviceMobileUri = overrideServiceMobileUri;                        
                    }

                    string customerId = (string)account.Element("CustomerId");
                    string secretKey = (string)account.Element("SecretKey");
                    teleSignConfig.Add("ServiceAddress", serviceUri);
                    teleSignConfig.Add("ServiceMobileAddress", serviceMobileUri);
                    //return new TeleSignServiceConfiguration(
                    //            new TeleSignCredential(Guid.Parse(customerId), secretKey),
                    //            new Uri(serviceUri),
                    //			new Uri(serviceMobileUri));
                    return new TeleSignServiceConfiguration(
                                new TeleSignCredential(Guid.Parse(customerId), secretKey),
                                teleSignConfig);
                }
            }

            string message = string.Format(
                        "There was no account '{0}' found in the configuration", 
                        accountName);

            throw new ArgumentException(message);
        }
       
        /// <summary>
        /// Helper method to determine the location of the executing assembly then
        /// construct the path of the config file in that directory.
        /// </summary>
        /// <returns>The path to the default configuration file.</returns>
        private static string GetDefaultConfigurationFilePath()
        {
            string assemblyFilePath = Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyFilePath);

            return Path.Combine(
                        assemblyDirectory, 
                        "TeleSign.config.xml");
        }
    }
}
