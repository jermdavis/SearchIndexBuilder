using CommandLine;
using SearchIndexBuilder.Processors.Deploy;
using SearchIndexBuilder.Processors.Indexing;
using SearchIndexBuilder.Processors.Remove;
using SearchIndexBuilder.Processors.Retry;
using SearchIndexBuilder.Processors.Setup;
using System;

namespace SearchIndexBuilder
{

    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"  _____                     _     _____           _           ");
            Console.WriteLine(@" / ____|   Sitecore        | |   |_   _|         | |          ");
            Console.WriteLine(@"| (___   ___  __ _ _ __ ___| |__   | |  _ __   __| | _____  _ ");
            Console.WriteLine(@" \___ \ / _ \/ _` | '__/ __| '_ \  | | | '_ \ / _` |/ _ \ \/ /");
            Console.WriteLine(@" ____) |  __/ (_| | | | (__| | | |_| |_| | | | (_| |  __/>  < ");
            Console.WriteLine(@"|_____/ \___|\__,_|_|  \___|_| |_|_____|_| |_|\__,_|\___/_/\_\");
            Console.WriteLine(@"                                                   Builder    ");

            var parser = new Parser(s => {
                s.HelpWriter = Console.Error;
                s.CaseSensitive = false;
            });

            parser
                .ParseArguments<DeployOptions, SetupOptions, IndexingOptions, RetryOptions,  RemoveOptions>(args)
                .WithParsed<DeployOptions>(o => DeployProcessor.RunProcess(o))
                .WithParsed<SetupOptions>(o => SetupProcessor.RunProcess(o))
                .WithParsed<IndexingOptions>(o => IndexingProcessor.RunProcess(o))
                .WithParsed<RetryOptions>(o => RetryProcessor.RunProcess(o))
                .WithParsed<RemoveOptions>(o => RemoveProcessor.RunProcess(o));
        }
    }

}