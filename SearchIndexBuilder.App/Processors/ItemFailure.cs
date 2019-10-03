using SearchIndexBuilder.App.EndpointProxies;
using System;

namespace SearchIndexBuilder.App.Processors
{

    public class ItemFailure
    {
        public DateTime At { get; set; }
        public ItemEntry Item { get; set; }
        public string[] Errors { get; set; }
        public FailureType FailureType { get; set; }
    }

}