using CommandLine;

namespace SearchIndexBuilder.Processors.Remove
{

    [Verb("remove", HelpText = "Remove the endpoint from the specified website")]
    public class RemoveOptions : CoreOptions
    {
        [Option('w', "website", Required = true, HelpText = "The website root folder to remove the endpoint file from")]
        public string Website { get; set; }
    }

}
