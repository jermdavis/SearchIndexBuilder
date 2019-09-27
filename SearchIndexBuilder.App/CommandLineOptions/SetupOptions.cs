using CommandLine;

namespace SearchIndexBuilder.App.CommandLineOptions
{

    [Verb("Setup", HelpText = "Fetches config data for a rebuild operation from a target server")]
    public class SetupOptions
    {
        [Option('u', "url", Required = true, HelpText = "The URL for the SearchIndexBuilder endpoint")]
        public string Url { get; set; }

        [Option('d', "database", Required = true, HelpText = "The database to process")]
        public string Database { get; set; }

        [Option('q', "query", Required = false, HelpText = "An optional Sitecore Query to specify which items to update")]
        public string Query { get; set; } = string.Empty;

        [Option('c', "config", Required = false, HelpText = "The file to write the config json to")]
        public string ConfigFile { get; set; } = "config.json";
    }

}