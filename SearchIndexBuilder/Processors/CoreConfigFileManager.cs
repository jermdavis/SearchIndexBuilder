using System;
using System.IO;

namespace SearchIndexBuilder.Processors
{
    public abstract class CoreConfigFileManager : IConfigFileManager
    {
        protected string _fileExtension;
        public abstract OperationConfig Load(string filename);

        public abstract void Save(string filename, OperationConfig config);

        public string VerifyFilename(string filename)
        {
            if (filename.EndsWith(_fileExtension, StringComparison.OrdinalIgnoreCase))
            {
                return filename;
            }
            else
            {
                return $"{filename}{_fileExtension}";
            }
        }

        public CoreConfigFileManager(string fileExtension)
        {
            if(fileExtension.StartsWith("."))
            {
                _fileExtension = fileExtension;
            }
            else
            {
                _fileExtension = $".{fileExtension}";
            }
        }

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

        public string RuntimeBackupFilename(string filename)
        {
            return "RuntimeBackup-" + filename;
        }

        public string Backup(string filename)
        {
            filename = VerifyFilename(filename);

            if (File.Exists(filename))
            {
                var file = filename;
                if(file.StartsWith("backup-"))
                {
                    file = file.Substring(23);
                }

                var now = DateTime.Now;
                var newFile = $"backup-{now.ToString("yyyyMMdd-HHmmss")}-{file}";

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
