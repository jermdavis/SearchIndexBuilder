using SearchIndexBuilder.App.CommandLineOptions;
using SearchIndexBuilder.App.EndpointProxies;
using System.Linq;

namespace SearchIndexBuilder.App.Processors.Setup
{

    public class SetupProcessor
    {
        public static void RunSetup(SetupOptions options, ISitecoreEndpointFactory endpointFactory)
        {
            var cfg = new OperationConfig();
            cfg.Url = options.Url;
            cfg.Database = options.Database;

            ISitecoreEndpoint endPoint = endpointFactory.Create(options.Url);

            cfg.Indexes = endPoint.FetchIndexes().ToArray();
            cfg.Items = endPoint.FetchItemIds(options.Database, options.Query);

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(cfg, Newtonsoft.Json.Formatting.Indented);

            using (var file = System.IO.File.CreateText(options.ConfigFile))
            {
                file.WriteLine(data);
            }
        }
    }

}