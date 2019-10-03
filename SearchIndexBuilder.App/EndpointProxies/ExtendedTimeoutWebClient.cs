using System;
using System.Net;

namespace SearchIndexBuilder.App.EndpointProxies
{
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