//-----------------------------------------------------------------------
// <copyright file="WebRequester.cs" company="TeleSign Corporation">
//     Copyright (c) TeleSign Corporation 2012.
// </copyright>
// <license>MIT</license>
// <author>Zentaro Kavanagh</author>
//-----------------------------------------------------------------------

namespace TeleSign.Services
{
    using System.IO;
    using System.Net;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Default implementation of IWebRequester using the built in .NET
    /// infrastructure.
    /// </summary>
    public class WebRequester : IWebRequester
    {
        /// <summary>
        /// Given a web request - reads the response and returns it all
        /// as a TeleSignResponse Object having JObject.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TSResponse ReadTeleSignResponse(WebRequest request)
        {
            TSResponse tsResponse = new TSResponse();
            try
           {
                request.Timeout = 30000;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    tsResponse.StatusLine = response.StatusDescription;
                    tsResponse.StatusCode = (int)response.StatusCode;
                    tsResponse.Body = response.ToString();
                    // Get the headers associated with Response
                    WebHeaderCollection headers = response.Headers;
                    for (int i = 0; i < headers.Count; ++i) {
                        tsResponse.addHeader(headers.GetKey(i), headers.GetValues(i));                       
                    }
                    tsResponse.Json = JObject.Parse(reader.ReadToEnd());
                    reader.Close();
                    response.Close();
                    return tsResponse;
                }
            }
            catch (WebException x)
            {
                // This error still has content in the body and was not really
                // a connection failure, but is a failure in what we sent to the
                // service.
                if (x.Status == WebExceptionStatus.ProtocolError)
                {
                    using (StreamReader reader = new StreamReader(x.Response.GetResponseStream()))
                    {
                        tsResponse.Body = reader.ReadToEnd();
                        tsResponse.Json = JObject.Parse(tsResponse.Body);
                        tsResponse.StatusCode = (int)((HttpWebResponse)x.Response).StatusCode;
                        // Get the headers associated with Response
                        WebHeaderCollection headers = x.Response.Headers;
                        for (int i = 0; i < headers.Count; ++i)
                        {
                            tsResponse.addHeader(headers.GetKey(i), headers.GetValues(i));
                        }
                        reader.Close();
                        return tsResponse;                        
                    }
                }                
                throw;
            }
        }
    }
}
