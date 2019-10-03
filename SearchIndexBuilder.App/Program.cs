using CommandLine;
using SearchIndexBuilder.App.EndpointProxies;
using SearchIndexBuilder.App.Processors.Deploy;
using SearchIndexBuilder.App.Processors.Indexing;
using SearchIndexBuilder.App.Processors.Remove;
using SearchIndexBuilder.App.Processors.Retry;
using SearchIndexBuilder.App.Processors.Setup;
using System;

namespace SearchIndexBuilder.App
{

    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"  _____                     _     _____           _           ");
            Console.WriteLine(@" / ____|                   | |   |_   _|         | |          ");
            Console.WriteLine(@"| (___   ___  __ _ _ __ ___| |__   | |  _ __   __| | _____  _ ");
            Console.WriteLine(@" \___ \ / _ \/ _` | '__/ __| '_ \  | | | '_ \ / _` |/ _ \ \/ /");
            Console.WriteLine(@" ____) |  __/ (_| | | | (__| | | |_| |_| | | | (_| |  __/>  < ");
            Console.WriteLine(@"|_____/ \___|\__,_|_|  \___|_| |_|_____|_| |_|\__,_|\___/_/\_\");
            Console.WriteLine(@"                                                       Builder");

            var parser = new Parser(s => {
                s.HelpWriter = Console.Error;
                s.CaseSensitive = false;
            });

            parser
                .ParseArguments<SetupOptions, IndexingOptions, DeployOptions, RemoveOptions, RetryOptions>(args)
                .WithParsed<SetupOptions>(o => SetupProcessor.RunProcess(o))
                .WithParsed<IndexingOptions>(o => ImprovedIndexingProcessor.RunProcess(o))
                .WithParsed<DeployOptions>(o => DeployProcessor.RunProcess(o))
                .WithParsed<RemoveOptions>(o => RemoveProcessor.RunProcess(o))
                .WithParsed<RetryOptions>(o => RetryProcessor.RunProcess(o));
        }
    }

}