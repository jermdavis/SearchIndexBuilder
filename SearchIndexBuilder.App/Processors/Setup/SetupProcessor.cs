using SearchIndexBuilder.App.EndpointProxies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SearchIndexBuilder.App.Processors.Setup
{

    /// <summary>
    /// Goes through the process of creating a config file for indexing
    /// </summary>
    public class SetupProcessor : BaseProcessor<SetupOptions>
    {
        public static void RunProcess(SetupOptions options, ISitecoreEndpointFactory endpointFactory)
        {
            var sp = new SetupProcessor(options, endpointFactory);
            sp.Run();
        }

        private ISitecoreEndpointFactory _endpointFactory;

        public SetupProcessor(SetupOptions options, ISitecoreEndpointFactory endpointFactory) : base(options)
        {
            _endpointFactory = endpointFactory;
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

            string data;
            try
            {
                Console.Write("Fetching indexes...");
                cfg.Indexes = endPoint.FetchIndexes(cfg.Token);
                Console.WriteLine();

                Console.Write("Fetching item data...");
                var items = endPoint.FetchItemIds(cfg.Token, _options.Database, _options.Query);
                cfg.Items = new Queue<ItemEntry>(items);
                Console.WriteLine();

                Console.Write("Serialising config...");
                data = Newtonsoft.Json.JsonConvert.SerializeObject(cfg, Newtonsoft.Json.Formatting.Indented);
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
            using (var file = File.CreateText(_options.ConfigFile))
            {
                file.WriteLine(data);
            }
            Console.WriteLine();

            Console.WriteLine($"Config written to {_options.ConfigFile}");
        }
    }

}