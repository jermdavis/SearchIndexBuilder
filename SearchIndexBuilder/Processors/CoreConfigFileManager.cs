using System;
using System.IO;

namespace SearchIndexBuilder.Processors
{
    public abstract class CoreConfigFileManager : IConfigFileManager
    {
        public abstract OperationConfig Load(string filename);

        public abstract void Save(string filename, OperationConfig config);

        protected OperationConfig ExtractConfig(StreamReader sr)
        {
            var data = sr.ReadToEnd();
            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<OperationConfig>(data);
            config.EnsureCollections();
            return config;
        }

        protected void WriteConfig(StreamWriter sw, OperationConfig config)
        {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
            sw.WriteLine(data);
            sw.Flush();
        }

        public long SizeOfSave(string filename)
        {
            var fi = new FileInfo(filename);

            return fi.Length;
        }

        public string Backup(string filename)
        {
            if (File.Exists(filename))
            {
                var now = DateTime.Now;
                var newFile = $"backup-{now.ToString("yyyyMMdd-hhmmss")}-{filename}";

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
