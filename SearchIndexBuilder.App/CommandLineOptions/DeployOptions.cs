using CommandLine;

namespace SearchIndexBuilder.App.CommandLineOptions
{

    [Verb("Deploy", HelpText = "Deploy the endpoint into ")]
    public class DeployOptions
    {
        [Option('w', "website", Required = true, HelpText = "The website root folder to deploy the endpoint file to")]
        public string Website { get; set; }
    }

}