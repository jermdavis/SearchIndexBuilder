using System;
using System.Net;

namespace SearchIndexBuilder.EndpointProxies
{

    /// <summary>
    /// Allows timeouts to be extended when making requests to the endpoint
    /// </summary>
    public class ExtendedTimeoutWebClient : WebClient
    {
        private int _timeout;

        public ExtendedTimeoutWebClient(int timeout)
        {
            _timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var wr = base.GetWebRequest(address);
            wr.Timeout = _timeout;
            return wr;
        }
    }

}