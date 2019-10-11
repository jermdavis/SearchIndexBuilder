using CommandLine;

namespace SearchIndexBuilder.Processors.Setup
{

    [Verb("setup", HelpText = "Fetches config data for a rebuild operation from a target server")]
    public class SetupOptions : CoreOptions
    {
        [Option('u', "url", Required = true, HelpText = "The URL for the SearchIndexBuilder endpoint")]
        public string Url { get; set; }

        [Option('d', "database", Required = true, HelpText = "The database to process")]
        public string Database { get; set; }

        [Option('q', "query", Required = false, HelpText = "An optional Sitecore Query to specify which items to update")]
        public string Query { get; set; } = string.Empty;

        [Option('c', "config", Required = false, HelpText = "The file to write the config json to")]
        public string ConfigFile { get; set; } = "config.json";

        [Option('t', "token", Required = true, HelpText = "Specify a security token to use when talking to the endpoint")]
        public string Token { get; set; }

        [Option('o', "overwrite", Required = false, HelpText = "If the config file exists, should it be overwritten?")]
        public bool Overwrite { get; set; } = false;

        [Option("timeout", Required = false, HelpText = "Allows you to specify a longer timeout for indexing operations. Expressed in seconds.")]
        public int Timeout { get; set; } = 60;
    }

}