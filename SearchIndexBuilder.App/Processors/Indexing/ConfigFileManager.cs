using System;
using System.IO;

namespace SearchIndexBuilder.App.Processors.Indexing
{
    public class ConfigFileManager
    {
        public OperationConfig Load(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                return null;
            }

            using (var file = System.IO.File.OpenText(filename))
            {
                var data = file.ReadToEnd();
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationConfig>(data);
                config.EnsureCollections();
                return config;
            }
        }

        public void Save(string filename, OperationConfig config)
        {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
            using (var file = File.CreateText(filename))
            {
                file.WriteLine(data);
            }
        }

        public string Backup(string filename)
        {
            if (File.Exists(filename))
            {
                var now = DateTime.Now;
                var newFile = $"backup-{now.ToString("yyyyMMdd-hhmm")}-{filename}";

                File.Move(filename, newFile);

                return newFile;
            }
            else
            {
                throw new InvalidOperationException("What should happen if file not found?");
                // What if file does not exist??
            }
        }
    }

}
