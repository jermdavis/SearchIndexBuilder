using SearchIndexBuilder.App.EndpointProxies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchIndexBuilder.App.Processors.Indexing
{

    public class ImprovedIndexingProcessor : IVerbProcessor
    {
        public static void RunProcess(IndexingOptions options, ISitecoreEndpointFactory endpointFactory)
        {
            var ip = new ImprovedIndexingProcessor(options, endpointFactory);
            ip.Run();
        }

        public ImprovedIndexingProcessor(IndexingOptions options, ISitecoreEndpointFactory endpointFactory)
        {
            Console.WriteLine("Running indexing");
            Console.WriteLine("----------------");

            var config = LoadConfiguration(options.ConfigFile);
            if(config == null)
            {
                return;
            }

            ISitecoreEndpoint endPoint = endpointFactory.Create(config.Url);
            var state = new ProcessState(endPoint, options, config);
            state.Config.Attempts += 1;
            var startedAt = DateTime.Now;

            ProcessAllGroups(state);

            var endedAt = DateTime.Now;
            var elapsed = endedAt - startedAt;
            state.Config.Elapsed += elapsed;

            SaveState(state);

            Console.WriteLine();
            Console.WriteLine(">>Finished");
            Console.WriteLine($">> at {endedAt} - after {elapsed}");
            
            Console.WriteLine($">> with {state.Config.Errors.Count()} errors");
            foreach (var err in state.Config.Errors)
            {
                Console.WriteLine(err);
            }

            Console.WriteLine($">> total time now {state.Config.Elapsed} after {state.Config.Attempts} attempts.");
        }

        private void ProcessAllGroups(ProcessState state)
        {
            state.Config.Processed.Enqueue(state.Config.Items.Dequeue());
            state.Config.Errors.Enqueue(new ItemError() { At = DateTime.Now, Item = state.Config.Items.Peek(), Errors = new string[] { "Test", "check" } });
            Console.WriteLine("processing...");
        }

        private void SaveState(ProcessState state)
        {
            MoveCurrentConfigToBackup(state.Options.ConfigFile);
            SaveCurrentConfig(state);
        }

        private void SaveCurrentConfig(ProcessState state)
        {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(state.Config, Newtonsoft.Json.Formatting.Indented);
            using (var file = File.CreateText(state.Options.ConfigFile))
            {
                file.WriteLine(data);
            }

            Console.WriteLine($">>Config saved to {state.Options.ConfigFile}");
        }

        private void MoveCurrentConfigToBackup(string configFile)
        {
            if(File.Exists(configFile))
            {
                var now = DateTime.Now;
                var newFile = $"backup-{now.ToString("yyyyMMdd-hhmm")}-{configFile}";

                File.Move(configFile, newFile);

                Console.WriteLine($">>Moved existing config {configFile} to {newFile}");
            }
            else
            {
                // What if file does not exist??
            }
        }

        private OperationConfig LoadConfiguration(string configFile)
        {
            if (!System.IO.File.Exists(configFile))
            {
                Console.WriteLine($"Error: Config file '{configFile}' not found");
                return null;
            }

            using (var file = System.IO.File.OpenText(configFile))
            {
                var data = file.ReadToEnd();
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationConfig>(data);
                config.EnsureCollections();
                return config;
            }
        }

        public void Run()
        {

        }
    }

}
