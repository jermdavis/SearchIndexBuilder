using System;
using System.IO;
using System.IO.Compression;

namespace SearchIndexBuilder.Processors
{

    public class ZipArchiveConfigFileManager : CoreConfigFileManager
    {
        public static readonly string FileExtension = ".zip";

        public ZipArchiveConfigFileManager() : base(FileExtension)
        {
        }

        public override OperationConfig Load(string filename)
        {
            using(var file = new FileStream(VerifyFilename(filename), FileMode.Open))
            {
                using(var archive = new ZipArchive(file, ZipArchiveMode.Read))
                {
                    var entry = archive.GetEntry(filename);

                    using(var entryStream = entry.Open())
                    {
                        using(var sr = new StreamReader(entryStream))
                        {
                            return base.ExtractConfig(sr);
                        }
                    }
                }
            }
        }

        public override void Save(string filename, OperationConfig config)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Update, true))
                {
                    var demoFile = archive.CreateEntry(filename, CompressionLevel.Optimal);

                    using (var entryStream = demoFile.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        base.WriteConfig(streamWriter, config);
                    }
                }

                using (var fileStream = new FileStream(VerifyFilename(filename), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }
    }

}
