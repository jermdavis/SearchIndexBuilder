using System;
using System.IO;

namespace SearchIndexBuilder.Processors
{

    /*
using (var memoryStream = new MemoryStream())
{
   using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
   {
      var demoFile = archive.CreateEntry("foo.txt");

      using (var entryStream = demoFile.Open())
      using (var streamWriter = new StreamWriter(entryStream))
      {
         streamWriter.Write("Bar!");
      }
   }

   using (var fileStream = new FileStream(@"C:\Temp\test.zip", FileMode.Create))
   {
      memoryStream.Seek(0, SeekOrigin.Begin);
      memoryStream.CopyTo(fileStream);
   }
}
     */

    /*
     string fileName = "export_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
    byte[] fileBytes = here is your file in bytes
    byte[] compressedBytes;
    string fileNameZip = "Export_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".zip";

    using (var outStream = new MemoryStream())
    {
        using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
        {
            var fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);
            using (var entryStream = fileInArchive.Open())
            using (var fileToCompressStream = new MemoryStream(fileBytes))
            {
                fileToCompressStream.CopyTo(entryStream);
            }
        }
        compressedBytes = outStream.ToArray();
    }
     */

    /// <summary>
    /// Reading and writing of config file data
    /// </summary>
    public class JsonConfigFileManager : IConfigFileManager
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
