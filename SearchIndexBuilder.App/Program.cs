using CommandLine;
using SearchIndexBuilder.App.EndpointProxies;
using SearchIndexBuilder.App.Processors.Deploy;
using SearchIndexBuilder.App.Processors.Indexing;
using SearchIndexBuilder.App.Processors.Remove;
using SearchIndexBuilder.App.Processors.Setup;
using System;

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

            //var endpointFactory = new FakeEndpointFactory();
            var endpointFactory = new SitecoreWebEndpointFactory();

            CommandLine.Parser.Default
                .ParseArguments<SetupOptions, IndexingOptions, DeployOptions, RemoveOptions>(args)
                .WithParsed<SetupOptions>(o => SetupProcessor.RunSetup(o, endpointFactory))
                .WithParsed<IndexingOptions>(o => IndexingProcessor.RunProcess(o, endpointFactory))
                .WithParsed<DeployOptions>(o => DeployProcessor.RunDeploy(o))
                .WithParsed<RemoveOptions>(o => RemoveProcessor.RunRemove(o));
        }
    }

}