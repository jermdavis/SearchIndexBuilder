using Newtonsoft.Json;
using SearchIndexBuilder.EndpointProxies;
using System;

namespace SearchIndexBuilder.Processors
{

    /// <summary>
    /// Error data model for the config file
    /// </summary>
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