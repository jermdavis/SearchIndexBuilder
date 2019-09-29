using System;

namespace SearchIndexBuilder.App.EndpointProxies
{

    /// <summary>
    /// Model type that reprents an Item in the config file
    /// </summary>
    public class ItemEntry
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }

}