using System.IO;
using System.IO.Compression;

namespace SearchIndexBuilder.Processors
{
    public class GZipStreamConfigFileManager : CoreConfigFileManager
    {
        public static readonly string FileExtension = ".gzip";

        public GZipStreamConfigFileManager() : base(FileExtension)
        {
        }

        public override OperationConfig Load(string filename)
        {
            using (var file = new FileStream(VerifyFilename(filename), FileMode.Open, FileAccess.Read))
            {
                using (var archive = new GZipStream(file, CompressionMode.Decompress, true))
                {
                    using (var sr = new StreamReader(archive))
                    {
                        return base.ExtractConfig(sr);
                    }
                }
            }
        }

        public override void Save(string filename, OperationConfig config)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new GZipStream(memoryStream, CompressionLevel.Optimal, true))
                {
                    using (var streamWriter = new StreamWriter(archive))
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
