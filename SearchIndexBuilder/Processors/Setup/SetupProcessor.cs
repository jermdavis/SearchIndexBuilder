using SearchIndexBuilder.EndpointProxies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SearchIndexBuilder.Processors.Setup
{

    /// <summary>
    /// Goes through the process of creating a config file for indexing
    /// </summary>
    public class SetupProcessor : BaseProcessor<SetupOptions>
    {
        public static void RunProcess(SetupOptions options)
        {
            var sp = new SetupProcessor(options);
            sp.Run();
        }

        public SetupProcessor(SetupOptions options) : base(options)
        {
        }

        public override void Run()
        {
            base.Run();

            Console.WriteLine("Writing configuration");
            Console.WriteLine("---------------------");

            if (File.Exists(_options.ConfigFile) && _options.Overwrite == false)
            {
                Console.WriteLine($"Config file {_options.ConfigFile} exists.");
                Console.WriteLine($"Overwrite not specified so file will not be overwritten");

                return;
            }

            var cfg = new OperationConfig();
            cfg.Url = _options.Url;
            cfg.Database = _options.Database;
            cfg.Token = _options.Token;

            ISitecoreEndpoint endPoint = _endpointFactory.Create(_options.Url);

            try
            {
                Console.Write("Fetching indexes...");
                cfg.Indexes = endPoint.FetchIndexes(cfg.Token);
                Console.WriteLine();

                Console.Write("Fetching item data...");
                var items = endPoint.FetchItemIds(cfg.Token, _options.Database, _options.Query);
                cfg.Items = new Queue<ItemEntry>(items);
                Console.WriteLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine(Environment.NewLine + "Exception caught from endpoint while extracting setup data:");
                while(ex != null)
                {
                    Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
                    //if(ex is System.Net.WebException)
                    //{
                    //    var s = (ex as System.Net.WebException).Response.GetResponseStream();
                    //    var sr = new StreamReader(s);
                    //    Console.WriteLine(sr.ReadToEnd());
                    //}
                    ex = ex.InnerException;
                }

                return;
            }

            Console.Write("Saving config to disk...");
            var cm = new ConfigFileManager();
            cm.Save(_options.ConfigFile, cfg);

            Console.WriteLine();
            Console.WriteLine($"Config written to {_options.ConfigFile}");
        }
    }

}