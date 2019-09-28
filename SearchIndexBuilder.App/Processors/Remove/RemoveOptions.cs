using CommandLine;

namespace SearchIndexBuilder.App.Processors.Remove
{
    [Verb("Remove", HelpText = "Remove the endpoint from the specified website")]
    public class RemoveOptions
    {
        [Option('w', "website", Required = true, HelpText = "The website root folder to remove the endpoint file from")]
        public string Website { get; set; }
    }

}
