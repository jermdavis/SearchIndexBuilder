using SearchIndexBuilder.EndpointProxies;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SearchIndexBuilder.Processors.Indexing
{

    /// <summary>
    /// Internal model for the current state of an indexing operation
    /// </summary>
    public class ProcessState
    {
        public Stopwatch sw { get; } = new Stopwatch();
        public ISitecoreEndpoint Endpoint { get; }
        public RollingAverage Average { get; } = new RollingAverage(50);
        public IndexingOptions Options { get; }
        public OperationConfig Config { get; }

        public ProcessState(ISitecoreEndpoint endPoint, IndexingOptions options, OperationConfig config)
        {
            Endpoint = endPoint;
            Options = options;
            Config = config;
        }
    }

}