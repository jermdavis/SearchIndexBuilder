using SearchIndexBuilder.App.EndpointProxies;
using System;
using System.IO;
using System.Linq;

namespace SearchIndexBuilder.App.Processors.Setup
{

    /// <summary>
    /// Goes through the process of creating a config file for indexing
    /// </summary>
    public class SetupProcessor
    {
        public static void RunSetup(SetupOptions options, ISitecoreEndpointFactory endpointFactory)
        {
            Console.WriteLine("Writing configuration");
            Console.WriteLine("---------------------");

            if (File.Exists(options.ConfigFile) && options.Overwrite == false)
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

            string data;
            try
            {
                cfg.Indexes = endPoint.FetchIndexes(cfg.Token);
                cfg.Items = endPoint.FetchItemIds(cfg.Token, options.Database, options.Query);

                data = Newtonsoft.Json.JsonConvert.SerializeObject(cfg, Newtonsoft.Json.Formatting.Indented);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception caught from endpoint while extracting setup data:");
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

            using (var file = File.CreateText(options.ConfigFile))
            {
                file.WriteLine(data);
            }

            Console.WriteLine($"Config written to {options.ConfigFile}");
        }
    }

}