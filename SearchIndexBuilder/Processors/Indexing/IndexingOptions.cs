﻿using CommandLine;

namespace SearchIndexBuilder.Processors.Indexing
{

    [Verb("index", HelpText = "Runs the index update defined in config")]
    public class IndexingOptions : CoreOptions
    {
        [Option('c', "config", Required = false, HelpText = "The file to write the config json to")]
        public string ConfigFile { get; set; } = "config.json";

        [Option('o', "outputEvery", Required = false, HelpText = "Show timing data every X items")]
        public int OutputEvery { get; set; } = 10;

        [Option('r', "retries", Required = false, HelpText = "How many times to retry an item before considering it failed")]
        public int Retries { get; set; } = 5;

        [Option('p', "pause", Required = false, HelpText = "To throttle the effect of the index build on your server, pause for this number of ms between operations.")]
        public int Pause { get; set; } = 0;

        [Option('t', "timeout", Required = false, HelpText = "Allows you to specify a longer timeout for indexing operations. Expressed in seconds.")]
        public int Timeout { get; set; } = -1;
    }

}