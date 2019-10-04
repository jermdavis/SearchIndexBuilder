using Newtonsoft.Json;
using System;

namespace SearchIndexBuilder.App.EndpointProxies
{

    /// <summary>
    /// Model type that reprents an Item in the config file
    /// </summary>
    public class ItemEntry
    {
        [JsonProperty(PropertyName = "n")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "i")]
        public Guid Id { get; set; }
    }

}