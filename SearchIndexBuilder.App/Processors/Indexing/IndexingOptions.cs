using CommandLine;

namespace SearchIndexBuilder.App.Processors.Indexing
{

    [Verb("Index", HelpText = "Runs the index update defined in config")]
    public class IndexingOptions
    {
        [Option('c', "config", Required = false, HelpText = "The file to write the config json to")]
        public string ConfigFile { get; set; } = "config.json";

        [Option('o', "outputEvery", Required = false, HelpText = "Show timing data every X items")]
        public int OutputEvery { get; set; } = 10;

        [Option('r', "retries", Required = false, HelpText = "How many times to retry an item before considering it failed")]
        public int Retries { get; set; } = 5;
    }

}