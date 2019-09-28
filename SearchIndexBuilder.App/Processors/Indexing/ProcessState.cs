using SearchIndexBuilder.App.EndpointProxies;
using System.Collections.Generic;
using System.Diagnostics;

namespace SearchIndexBuilder.App.Processors.Indexing
{

    public class ProcessState
    {
        public Stopwatch sw { get; } = new Stopwatch();
        public ISitecoreEndpoint Endpoint { get; }
        public RollingAverage Average { get; } = new RollingAverage(50);
        public Queue<ItemEntry> Items { get; }
        public IndexingOptions Options { get; }
        public OperationConfig Config { get; }

        public List<string> Errors { get; } = new List<string>();

        public ProcessState(ISitecoreEndpoint endPoint, Queue<ItemEntry> items, IndexingOptions options, OperationConfig config)
        {
            Endpoint = endPoint;
            Items = items;
            Options = options;
            Config = config;
        }
    }

}