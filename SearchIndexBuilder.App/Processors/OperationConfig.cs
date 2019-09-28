using SearchIndexBuilder.App.EndpointProxies;
using System.Collections.Generic;

namespace SearchIndexBuilder.App.Processors
{

    public class OperationConfig
    {
        public string Url { get; set; }
        public string Token { get; set; }
        public string Database { get; set; }
        public IEnumerable<string> Indexes { get; set; }
        public IEnumerable<ItemEntry> Items { get; set; }
    }

}