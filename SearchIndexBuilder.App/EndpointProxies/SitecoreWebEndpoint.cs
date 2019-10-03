using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace SearchIndexBuilder.App.EndpointProxies
{

    public class SitecoreWebEndpointFactory : ISitecoreEndpointFactory
    {
        public ISitecoreEndpoint Create(string url)
        {
            return new SitecoreWebEndpoint(url);
        }
    }

    /// <summary>
    /// The real endpoint type - makes web requests to the endpoint file deployed to Sitecore
    /// </summary>
    public class SitecoreWebEndpoint : ISitecoreEndpoint
    {
        private string _url;

        public SitecoreWebEndpoint(string url)
        {
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            _url = url + Constants.EndpointFile;
        }

        public IEnumerable<string> FetchDatabases(string token)
        {
            WebClient wc = new WebClient();
            var data = wc.DownloadString($"{_url}?cmd=dbs&t={token}");

            return Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(data);
        }

        public IEnumerable<string> FetchIndexes(string token)
        {
            WebClient wc = new WebClient();
            var data = wc.DownloadString($"{_url}?cmd=indexes&t={token}");

            return Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(data);
        }

        public IEnumerable<ItemEntry> FetchItemIds(string token, string database, string query)
        {
            WebClient wc = new WebClient();
            var data = wc.DownloadString($"{_url}?cmd=items&db={database}&t={token}&q={HttpUtility.UrlEncode(query)}");

            return Newtonsoft.Json.JsonConvert.DeserializeObject<ItemEntry[]>(data);
        }

        public IndexResult IndexItem(string token, Guid id, string databaseName, IEnumerable<string> indexes, int timeout)
        {
            WebClient wc = new ExtendedTimeoutWebClient(timeout);

            var idx = string.Join(",", indexes);

            var data = wc.DownloadString($"{_url}?cmd=process&db={databaseName}&idx={idx}&id={id}&t={token}&to={timeout}");

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Activity[]>(data);

            return new IndexResult(result);
        }
    }

}