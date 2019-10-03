using Newtonsoft.Json;
using SearchIndexBuilder.App.EndpointProxies;
using System;
using System.Collections.Generic;

namespace SearchIndexBuilder.App.Processors
{

    /// <summary>
    /// Model class for the config data saved to disk
    /// </summary>
    public class OperationConfig
    {
        public string Url { get; set; }
        public string Token { get; set; }
        public string Database { get; set; }
        public IEnumerable<string> Indexes { get; set; }
        public Queue<ItemEntry> Items { get; set; }
        public Queue<ItemEntry> Processed { get; set; }
        public Queue<ItemError> Errors { get; set; }

        [JsonIgnore]
        public int TotalItems { get { return Items.Count + Processed.Count; } }
        public TimeSpan Elapsed { get; set; } = TimeSpan.Zero;
        public int Attempts { get; set; }

        public void EnsureCollections()
        {
            if(Processed == null)
            {
                Processed = new Queue<ItemEntry>();
            }
            if(Errors == null)
            {
                Errors = new Queue<ItemError>();
            }
        }
    }

}