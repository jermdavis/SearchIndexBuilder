using System.IO;

namespace SearchIndexBuilder.Processors
{

    /// <summary>
    /// Reading and writing of config file data
    /// </summary>
    public class TextConfigFileManager : CoreConfigFileManager
    {
        public TextConfigFileManager() : base(".json")
        {
        }

        public override OperationConfig Load(string filename)
        {
            filename = VerifyFilename(filename);

            if (!System.IO.File.Exists(filename))
            {
                return null;
            }

            using (var file = System.IO.File.OpenText(filename))
            {
                return ExtractConfig(file);
            }
        }

        public override void Save(string filename, OperationConfig config)
        {
            using (StreamWriter file = File.CreateText(VerifyFilename(filename)))
            {
                WriteConfig(file, config);
            }
        }   
    }
}
