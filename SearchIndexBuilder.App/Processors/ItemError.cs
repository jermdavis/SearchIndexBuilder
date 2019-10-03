using SearchIndexBuilder.App.EndpointProxies;
using System;

namespace SearchIndexBuilder.App.Processors
{
    public class ItemError
    {
        public DateTime At { get; set; }
        public ItemEntry Item { get; set; }
        public string[] Errors { get; set; }
    }

}