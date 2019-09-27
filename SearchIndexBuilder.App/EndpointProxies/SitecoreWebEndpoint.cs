using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SearchIndexBuilder.App.EndpointProxies
{

    public class SitecoreWebEndpoint : ISitecoreEndpoint
    {
        private string _url;

        public SitecoreWebEndpoint(string url)
        {
            _url = url;
        }

        public IEnumerable<string> FetchDatabases()
        {
            WebClient wc = new WebClient();
            var data = wc.DownloadString($"{_url}?cmd=dbs");

            return Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(data);
        }

        public IEnumerable<string> FetchIndexes()
        {
            WebClient wc = new WebClient();
            var data = wc.DownloadString($"{_url}?cmd=indexes");

            return Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(data);
        }

        public IEnumerable<ItemEntry> FetchItemIds(string database, string query)
        {
            // do something with the query!

            WebClient wc = new WebClient();
            var data = wc.DownloadString($"{_url}?cmd=items&db={database}");

            return Newtonsoft.Json.JsonConvert.DeserializeObject<ItemEntry[]>(data);
        }

        public bool IndexItem(Guid id, string databaseName, IEnumerable<string> indexes)
        {
            WebClient wc = new WebClient();

            var idx = string.Join(",", indexes);

            var data = wc.DownloadString($"{_url}?cmd=process&db={databaseName}&idx={idx}&id={id}");

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Activity[]>(data);
            var errors = result.Where(a => a.Error != null).Any();

            return !errors;
        }
    }

}