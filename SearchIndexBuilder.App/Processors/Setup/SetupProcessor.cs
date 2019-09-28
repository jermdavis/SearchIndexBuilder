using SearchIndexBuilder.App.EndpointProxies;
using System;
using System.IO;
using System.Linq;

namespace SearchIndexBuilder.App.Processors.Setup
{

    public class SetupProcessor
    {
        public static void RunSetup(SetupOptions options, ISitecoreEndpointFactory endpointFactory)
        {
            if(File.Exists(options.ConfigFile) && options.Overwrite == false)
            {
                Console.WriteLine($"Config file {options.ConfigFile} exists.");
                Console.WriteLine($"Overwrite not specified so file will not be overwritten");

                return;
            }

            var cfg = new OperationConfig();
            cfg.Url = options.Url;
            cfg.Database = options.Database;
            cfg.Token = options.Token;

            ISitecoreEndpoint endPoint = endpointFactory.Create(options.Url);

            cfg.Indexes = endPoint.FetchIndexes(cfg.Token).ToArray();
            cfg.Items = endPoint.FetchItemIds(cfg.Token, options.Database, options.Query);

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(cfg, Newtonsoft.Json.Formatting.Indented);

            using (var file = File.CreateText(options.ConfigFile))
            {
                file.WriteLine(data);
            }

            Console.WriteLine($"Config written to {options.ConfigFile}");
        }
    }

}