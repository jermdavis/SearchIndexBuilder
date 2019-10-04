using Newtonsoft.Json;
using SearchIndexBuilder.App.EndpointProxies;
using System;

namespace SearchIndexBuilder.App.Processors
{

    public class ItemFailure
    {
        [JsonProperty(PropertyName = "t")]
        public DateTime At { get; set; }
        [JsonProperty(PropertyName = "i")]
        public ItemEntry Item { get; set; }
        [JsonProperty(PropertyName = "e")]
        public string[] Errors { get; set; }
        [JsonProperty(PropertyName = "f")]
        public FailureType FailureType { get; set; }
    }

}