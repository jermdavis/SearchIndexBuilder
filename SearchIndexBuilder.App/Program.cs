using CommandLine;
using SearchIndexBuilder.App.CommandLineOptions;
using SearchIndexBuilder.App.EndpointProxies;
using SearchIndexBuilder.App.Processors;
using SearchIndexBuilder.App.Processors.Deploy;
using SearchIndexBuilder.App.Processors.Indexing;
using SearchIndexBuilder.App.Processors.Setup;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SearchIndexBuilder.App
{

    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SearchIndexBuilder");
            Console.WriteLine("------------------");
            Console.WriteLine();

            var parser = new CommandLine.Parser(s => {
                s.AutoHelp = true;
                s.CaseSensitive = false;
            });

            var endpointFactory = new FakeEndpointFactory();

            CommandLine.Parser.Default
                .ParseArguments<SetupOptions, IndexingOptions, DeployOptions>(args)
                .WithParsed<SetupOptions>(o => SetupProcessor.RunSetup(o, endpointFactory))
                .WithParsed<IndexingOptions>(o => IndexingProcessor.RunProcess(o, endpointFactory))
                .WithParsed<DeployOptions>(o => DeployProcessor.RunDeploy(o));
        }
    }

}