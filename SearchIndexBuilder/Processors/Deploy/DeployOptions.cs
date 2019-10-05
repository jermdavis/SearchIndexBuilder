using CommandLine;

namespace SearchIndexBuilder.Processors.Deploy
{

    [Verb("deploy", HelpText = "Deploy the endpoint into a site")]
    public class DeployOptions : CoreOptions
    {
        [Option('w', "website", Required = true, HelpText = "The website root folder to deploy the endpoint file to")]
        public string Website { get; set; }

        [Option('o', "overwrite", Required = false, HelpText = "If the endpoint file exists, should it be overwritten?")]
        public bool Overwrite { get; set; } = false;

        [Option('t', "token", Required = false, HelpText = "Specify a security token to use, rather than a random one")]
        public string Token { get; set; }
    }

}